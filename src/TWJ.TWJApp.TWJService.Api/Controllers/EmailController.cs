using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.Email.Commands.Send;
using TWJ.TWJApp.TWJService.Application.Services.Email.Commands.Unsubscribe;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmailController : BaseController
    {
        #region Send
        [HttpGet("Send")]
        public async Task<IActionResult> Send([FromQuery] SendEmailCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Send

        #region Unsubscribe
        [HttpPost("Unsubscribe")]
        [AllowAnonymous]
        public async Task<IActionResult> Unsubscribe([FromBody] UnsubscribeEmailCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Unsubscribe
    }
}
