using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;

namespace Maestro.api.Controllers.Extend
{
    public abstract class ControllerBaseExtended : ControllerBase
    {
        /// <summary>
        /// Error code 500 with default message an error is occured!
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public ObjectResult InternalServerError()
            => InternalServerError("An error is occured !");

        /// <summary>
        /// Error code 500 with custom message
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [NonAction]
        public ObjectResult InternalServerError([ActionResultObjectValue] object? value)
            => base.StatusCode((int)HttpStatusCode.InternalServerError, value);
    }
}
