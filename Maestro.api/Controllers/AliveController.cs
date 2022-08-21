using Maestro.api.Managers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Maestro.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AliveController : ControllerBase
    {
        private readonly RedisManager redisManager;
        public AliveController(RedisManager redisManager)
        {
            this.redisManager = redisManager;
        }

        [HttpGet, ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public string Ping()
        {
            return  redisManager.Ping();
        }
    }
}
