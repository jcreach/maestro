using Maestro.api.Extensions;
using Maestro.api.Models;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Maestro.api.Managers
{
    public class RedisManager
    {
        private readonly ILogger<RedisManager> logger;
        private readonly IConnectionMultiplexer redisClient;
        private readonly IDatabase db;
        private long applicationListLength;
        private readonly KeysManager keysManager;

        public RedisManager(IConnectionMultiplexer redisClient, ILogger<RedisManager> logger, KeysManager keysManager)
        {
            this.redisClient = redisClient;
            db = this.redisClient.GetDatabase();
            this.logger = logger;
            this.keysManager = keysManager;
        }

        /// <summary>
        /// Check if Maestro is currently availlable
        /// </summary>
        /// <returns>The observed latency or empty if not availlable</returns>
        public string Ping()
        {
            try
            {
                var latency = db.Ping().ToString();
                this.logger.Debug($"Redis ping {latency}");
                return latency;
            }
            catch (Exception ex)
            {
                this.logger.Error(ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Check if the key exist
        /// </summary>
        /// <param name="key">Redis key</param>
        /// <returns>true or false</returns>
        public async Task<bool> ExistAsync(string key)
            => await db.KeyExistsAsync(key);

        /// <summary>
        /// Return the application information for the specified key
        /// </summary>
        /// <param name="applicationKey">application key</param>
        /// <returns>application information for the specified key or null</returns>
        public async Task<ApplicationInformationsExtended?> GetApplicationInformationsAsync(string applicationKey)
        {
            try
            {
                string? resultApplicationInformation = await db.StringGetAsync(applicationKey);

                if (string.IsNullOrWhiteSpace(resultApplicationInformation))
                    return null;

                return JsonSerializer.Deserialize<ApplicationInformationsExtended>(resultApplicationInformation);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Return the configuration for the specified application
        /// </summary>
        /// <param name="applicationKey">application key</param>
        /// <returns>configuration for the specified application</returns>
        public async Task<string> GetConfigurationAsync(string applicationKey)
        {
            try
            {
                if (!await ExistAsync(applicationKey))
                    return string.Empty;

                var application = await GetApplicationInformationsAsync(applicationKey);

                if (application is null || !await ExistAsync(application.ConfigurationKey))
                    return string.Empty;

                string? result = await db.StringGetAsync(application.ConfigurationKey);

                return result ?? string.Empty;
            }
            catch (Exception ex)
            {
                this.logger.Error(ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Register an application
        /// </summary>
        /// <param name="applicationInformations">Application name, version, etc...</param>
        /// <returns>true if the application if properly registered else false</returns>
        public async Task<bool> RegisterApplicationAsync(ApplicationInformations applicationInformations)
        {
            try
            {
                var result = await db.StringSetAsync(applicationInformations.Key, JsonSerializer.Serialize(applicationInformations));
                var newListLength = await db.ListRightPushAsync(Constants.applicationKeysName, applicationInformations.Key);
                var isPushSuccess = applicationListLength < newListLength;

                if (isPushSuccess)
                    applicationListLength = newListLength;

                return result && isPushSuccess;
            }
            catch (Exception ex)
            {
                this.logger.Error(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Store the configuration for the specified application
        /// </summary>
        /// <param name="applicationKey">application key</param>
        /// <param name="configuration">configuration to store</param>
        /// <returns>true if the configuration if properly stored else false</returns>
        public async Task<bool> StoreConfigurationAsync(string applicationKey, string configuration)
        {
            if (!await ExistAsync(applicationKey))
                return false;

            try
            {
                string? applicationValue = await db.StringGetAsync(applicationKey);

                if (string.IsNullOrWhiteSpace(applicationValue))
                    return false;

                var application = JsonSerializer.Deserialize<ApplicationInformationsExtended>(applicationValue);

                if (application is null)
                    return false;

                if (string.IsNullOrWhiteSpace(application.ConfigurationKey))
                {
                    application.ConfigurationKey = this.keysManager.GenerateConfigurationKey();
                    applicationValue = await db.StringSetAndGetAsync(applicationKey, JsonSerializer.Serialize(application));

                    if (!string.IsNullOrWhiteSpace(applicationValue))
                        application = JsonSerializer.Deserialize<ApplicationInformationsExtended>(applicationValue);
                }

                return await db.StringSetAsync(application?.ConfigurationKey, configuration);
            }
            catch(Exception ex)
            {
                this.logger.Error(ex.Message);
                return false;
            }
        }
    }
}
