
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Commands.Add;
using TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Commands.Delete;
using TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Commands.Update;
using TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsLetterSubscriberController : BaseController
    {       
        #region Get-All
        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery]GetAllNewsLetterSubscriberQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Get-All

        #region Add
        [HttpPost("Add")]
        [AllowAnonymous]
        public async Task<IActionResult> Add([FromBody] AddNewsLetterSubscriberCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Add

        #region Update
        [HttpPut("Update")]
        [AllowAnonymous]
        public async Task<IActionResult> Update([FromBody] UpdateNewsLetterSubscriberCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Update

        #region Delete
        [HttpDelete("Delete")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete([FromBody] DeleteNewsLetterSubscriberCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Delete
    }
}