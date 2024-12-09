
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.Banner.Commands.Add;
using TWJ.TWJApp.TWJService.Application.Services.Banner.Commands.Delete;
using TWJ.TWJApp.TWJService.Application.Services.Banner.Commands.Update;
using TWJ.TWJApp.TWJService.Application.Services.Banner.Queries.GetAll;
using TWJ.TWJApp.TWJService.Application.Services.Banner.Queries.GetRandom;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BannerController : BaseController
    {       
        #region Get-All
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery]GetAllBannerQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Get-All

        #region GetRandom
        [HttpGet("GetRandom")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRandom([FromQuery] GetRandomBannerQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion GetRandom

        #region Add
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] AddBannerCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Add

        #region Update
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateBannerCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Update

        #region Delete
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteBannerCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Delete
    }
}