using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.AdClick.Queries.GetAll;
using TWJ.TWJApp.TWJService.Application.Services.User.Queries.GetById;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class UserController : BaseController
    {
        #region Get-All
        [HttpGet("GetByAuthorName")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] GetAllAdClickQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Get-All

        #region GetById
        [HttpGet("GetById")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById([FromQuery] GetUserByIdQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion GetById
    }
}
