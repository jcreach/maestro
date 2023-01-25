using Maestro.api.Controllers.Extend;
using Maestro.api.Managers;
using Maestro.api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using Maestro.api.Extensions;

namespace Maestro.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ApplicationController : ControllerBaseExtended
    {
        private readonly ILogger<ApplicationController> logger;
        private readonly RedisManager redisManager;
        private readonly KeysManager keysManager;

        public ApplicationController(ILogger<ApplicationController> logger, RedisManager redisManager, KeysManager keysManager)
        {
            this.logger = logger;
            this.redisManager = redisManager;
            this.keysManager = keysManager;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Register([FromBody] ApplicationInformations applicationInformations)
        {   
            if (applicationInformations is null)
                return BadRequest();
            try
            {
                applicationInformations.Key = keysManager.GenerateApplicationKey();

                if (this.redisManager.Ping().Equals(TimeSpan.Zero) || string.IsNullOrWhiteSpace(applicationInformations.Key))
                    return InternalServerError();

                if (await this.redisManager.RegisterApplicationAsync(applicationInformations))
                    return Ok(applicationInformations.Key);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex.Message);
            }
            return InternalServerError();
        }


        [HttpGet("{applicationKey}")]
        [ProducesResponseType(typeof(ApplicationInformationsExtended),(int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetApplicationInformations([FromRoute] string applicationKey)
        {
            if(string.IsNullOrWhiteSpace(applicationKey))
                return BadRequest();

            var result = await this.redisManager.GetApplicationInformationsAsync(applicationKey);
            if(result is null)
                return NotFound();
            return Ok(result);
        }
    }
}
