using Business.Handlers.Users.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : BaseApiController
    {
        /// <summary>
        /// Registers or updates the user's FCM device token
        /// </summary>
        [Consumes("application/json")]
        [HttpPost("register-token")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> RegisterToken([FromBody] RegisterFcmTokenCommand command)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(command));
        }
    }
}
