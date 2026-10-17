using System.Threading.Tasks;
using Business.Handlers.Contact.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>İletişim formu (SMTP ile support@masavtech.com).</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : BaseApiController
    {
        /// <summary>
        /// İletişim formu mesajı gönderir (şikayet, öneri, geri bildirim vb.)
        /// </summary>
        /// <param name="command">İletişim formu verileri</param>
        /// <returns>Sonuç mesajı</returns>
        [Authorize]
        [Consumes("application/json")]
        [HttpPost("send")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> SendContactMessage([FromBody] SendContactMessageCommand command)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(command));
        }

        /// <summary>
        /// Kurum kayıt / demo talebi gönderir (vhas_ar@hotmail.com adresine).
        /// Halka açıktır, oturum açma gerektirmez.
        /// </summary>
        /// <param name="command">Kurum başvuru verileri</param>
        /// <returns>Başvuru sonuç mesajı</returns>
        [AllowAnonymous]
        [Consumes("application/json")]
        [HttpPost("institution-registration")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> SendInstitutionRegistration([FromBody] SendInstitutionRegistrationCommand command)
        {
            return GetResponseOnlyResultMessage(await Mediator.Send(command));
        }
    }
}
