
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.Category.Commands.Add;
using TWJ.TWJApp.TWJService.Application.Services.Category.Commands.Delete;
using TWJ.TWJApp.TWJService.Application.Services.Category.Commands.Update;
using TWJ.TWJApp.TWJService.Application.Services.Category.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : BaseController
    {       
        #region Get-All
        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery]GetAllCategoryQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Get-All

        #region Add
        [HttpPost("Add")]
        [AllowAnonymous]
        public async Task<IActionResult> Add([FromBody] AddCategoryCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Add


        #region Update
        [HttpPut("Update")]
        [AllowAnonymous]
        public async Task<IActionResult> Update([FromBody] UpdateCategoryCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Update


        #region Delete
        [HttpDelete("Delete")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete([FromBody] DeleteCategoryCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Delete
    }
}