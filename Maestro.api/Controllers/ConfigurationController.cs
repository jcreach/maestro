using Maestro.api.Controllers.Extend;
using Maestro.api.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Maestro.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBaseExtended
    {
        private readonly RedisManager redisManager;
        private readonly KeysManager keysManager;
        public ConfigurationController(RedisManager redisManager, KeysManager keysManager)
        {
            this.redisManager = redisManager;
            this.keysManager = keysManager;
        }

        [HttpGet("{applicationKey}")]
        public async Task<IActionResult> GetConfiguration(string applicationKey)
        {
            if (string.IsNullOrEmpty(applicationKey))
                return BadRequest();

            var result = await this.redisManager.GetConfigurationAsync(applicationKey);
            return Ok(result);
        }

        [HttpPost("{applicationKey}")]
        public async Task<IActionResult> SetConfigurationAsync([FromRoute]string applicationKey, [FromBody]string configuration)
        {
            if(string.IsNullOrWhiteSpace(applicationKey) || string.IsNullOrWhiteSpace(configuration))
                return BadRequest();

            var result = await this.redisManager.StoreConfigurationAsync(applicationKey, configuration);

            return Ok(result);
        }
    }
}
