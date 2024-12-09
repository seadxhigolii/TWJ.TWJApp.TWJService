using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.Account.Commands.Login;
using TWJ.TWJApp.TWJService.Application.Services.Account.Commands.Logout;
using TWJ.TWJApp.TWJService.Application.Services.Account.Commands.Register;
using TWJ.TWJApp.TWJService.Application.Services.Instagram.Commands.Add;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    [Authorize]
    public class AuthController : BaseController
    {
        #region Login
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginAccountCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion

        #region Register
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterAccountCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion

        #region Logout
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutAccountCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Logout
    }
}
