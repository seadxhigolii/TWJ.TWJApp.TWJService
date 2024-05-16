using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.Add;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.Delete;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.Generate;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.GenerateRandom;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.Update;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetAll;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetByTagName;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetByUrl;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetRelated;
using TWJ.TWJApp.TWJService.Application.Services.Template.Commands.Delete;
using TWJ.TWJApp.TWJService.Application.Services.Template.Commands.Update;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostController : BaseController
    {
        #region Get-All
        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] GetAllBlogPostQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Get-All

        #region GetByUrl
        [HttpGet("GetByUrl")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByUrl([FromQuery] GetBlogPostByUrlQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion GetByUrl

        #region GetRelated
        [HttpGet("GetRelated")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRelated([FromQuery] GetRelatedBlogPostQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion GetRelated

        #region GetByTagName
        [HttpGet("GetByTagName")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByTagName([FromQuery] GetBlogPostByTagNameQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion GetByTagName

        #region Add
        [HttpPost("Add")]
        [AllowAnonymous]
        public async Task<IActionResult> Add([FromBody] AddBlogPostCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Add

        #region Generate
        [AllowAnonymous]
        [HttpPost("Generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateBlogPostCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Generate

        #region GenerateSEOFocused
        [AllowAnonymous]
        [HttpPost("GenerateSEOFocused")]
        public async Task<IActionResult> GenerateSEOFocused([FromBody] GenerateSEOFocuedBlogPostCommand command,CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion GenerateSEOFocused

        #region GenerateRandom
        [AllowAnonymous]
        [HttpPost("GenerateRandom")]
        public async Task<IActionResult> GenerateRandom([FromBody] GenerateRandomBlogPostCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion GenerateRandom

        #region Update
        [HttpPut("Update")]
        [AllowAnonymous]
        public async Task<IActionResult> Update([FromBody] UpdateBlogPostCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Update

        #region Delete
        [HttpDelete("Delete")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete([FromBody] DeleteBlogPostCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Delete  

    }
}
