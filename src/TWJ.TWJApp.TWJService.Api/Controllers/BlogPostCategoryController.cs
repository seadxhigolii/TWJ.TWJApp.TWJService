
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.BlogPostCategory.Commands.Add;
using TWJ.TWJApp.TWJService.Application.Services.BlogPostCategory.Commands.Delete;
using TWJ.TWJApp.TWJService.Application.Services.BlogPostCategory.Commands.Update;
using TWJ.TWJApp.TWJService.Application.Services.BlogPostCategory.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BlogPostCategoryController : BaseController
    {       
        #region Get-All
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery]GetAllBlogPostCategoryQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Get-All

        #region Add
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] AddBlogPostCategoryCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Add

        #region Update
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateBlogPostCategoryCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Update

        #region Delete
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteBlogPostCategoryCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Delete
    }
}