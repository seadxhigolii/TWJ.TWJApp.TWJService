
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
//using TWJ.TWJApp.TWJService.Application.Services.Tag.Commands.Add;
//using TWJ.TWJApp.TWJService.Application.Services.Tag.Commands.Delete;
//using TWJ.TWJApp.TWJService.Application.Services.Tag.Commands.Update;
using TWJ.TWJApp.TWJService.Application.Services.Tag.Queries.GetAll;
using TWJ.TWJApp.TWJService.Application.Services.Tag.Queries.GetFiltered;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TagController : BaseController
    {       
        #region Get-All
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery]GetAllTagQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Get-All  
        
        #region GetFiltered
        [HttpGet("GetFiltered")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFiltered([FromQuery] GetFilteredTagQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion GetFiltered
    }
}