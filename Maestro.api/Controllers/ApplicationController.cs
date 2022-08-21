using Maestro.api.Controllers.Extend;
using Maestro.api.Managers;
using Maestro.api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Maestro.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ApplicationController : ControllerBaseExtended
    {
        private readonly RedisManager redisManager;
        private readonly KeysManager keysManager;

        public ApplicationController(RedisManager redisManager, KeysManager keysManager)
        {
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

                if (await this.redisManager.RegisterApplication(applicationInformations))
                    return Ok(applicationInformations.Key);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            return InternalServerError();
        }


        [HttpGet("{applicationKey}")]
        public async Task<IActionResult> GetApplicationInformations([FromRoute] string applicationKey)
        {
            if(string.IsNullOrWhiteSpace(applicationKey))
                return BadRequest();

            var result = await this.redisManager.GetApplicationInformations(applicationKey);
            if(result is null)
                return NotFound();
            return Ok(result);
        }
    }
}
