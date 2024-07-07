
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.News.Commands.Add;
using TWJ.TWJApp.TWJService.Application.Services.News.Commands.Delete;
using TWJ.TWJApp.TWJService.Application.Services.News.Commands.Generate;
using TWJ.TWJApp.TWJService.Application.Services.News.Commands.Update;
using TWJ.TWJApp.TWJService.Application.Services.News.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NewsController : BaseController
    {       
        #region Get-All
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery]GetAllNewsQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Get-All

        #region Add
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] AddNewsCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Add

        #region Generate
        [HttpPost("Generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateNewsCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Generate

        #region Update
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateNewsCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Update

        #region Delete
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteNewsCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Delete
    }
}