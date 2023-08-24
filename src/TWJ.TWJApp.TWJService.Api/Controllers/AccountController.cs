using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.Account.Commands.Login;
using TWJ.TWJApp.TWJService.Application.Services.Account.Commands.Register;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : BaseController
    {
        [AllowAnonymous]
        [HttpPost(":login")]
        public async Task<IActionResult> Login([FromBody] LoginAccountCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost(":register")]
        public async Task<IActionResult> Register([FromBody] RegisterAccountCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
    }
}
