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

        public RedisManager(IConnectionMultiplexer redisClient, ILogger<RedisManager> logger)
        {
            this.redisClient = redisClient;
            db = this.redisClient.GetDatabase();
            this.logger = logger;
        }

        public string Ping()
        {
            try
            {
                return db.Ping().ToString();
            }
            catch (Exception ex)
            {
                this.logger.Error(ex.Message);
                return string.Empty;
            }
        }

        public async Task<bool> RegisterApplication(ApplicationInformations applicationInformations)
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
            catch(Exception ex)
            {
                this.logger.Error(ex.Message);
                return false;
            }
        }

        public async Task<ApplicationInformationsExtended?> GetApplicationInformations(string applicationKey)
        {
            try
            {
                var resultApplicationInformation = await db.StringGetAsync(applicationKey);

                var stringConfig = string.Empty;
                if (resultApplicationInformation.HasValue)
                    stringConfig = (string?)resultApplicationInformation;

                if (string.IsNullOrWhiteSpace(stringConfig))
                    return null;

                return JsonSerializer.Deserialize<ApplicationInformationsExtended>(stringConfig);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex.Message);
                return null;
            }
        }
    }
}
