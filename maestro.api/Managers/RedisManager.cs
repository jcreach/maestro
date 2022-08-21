using Maestro.api.Extensions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;

namespace Maestro.api.Managers
{
    public class RedisManager
    {
        private readonly ILogger<RedisManager> logger;
        private readonly IConnectionMultiplexer redisClient;
        private readonly IDatabase db;
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
    }
}
