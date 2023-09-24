using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Add;
using TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Delete;
using TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Update;
using TWJ.TWJApp.TWJService.Application.Services.Template.Commands.Add;
using TWJ.TWJApp.TWJService.Application.Services.Template.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TemplateSettingController : BaseController
    {
        //#region Get-All
        //[HttpGet("GetAll")]
        //[AllowAnonymous]
        //public async Task<IActionResult> GetAll([FromBody] GetAllTemplatesQuery command, CancellationToken cancellation)
        //{
        //    var result = await Mediator.Send(command, cancellation);

        //    return Ok(result);
        //}
        //#endregion Get-All

        #region Add
        [HttpPost("Add")]
        [AllowAnonymous]
        public async Task<IActionResult> Add([FromBody] AddTemplateSettingCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Add


        #region Update
        [HttpPut("Update")]
        [AllowAnonymous]
        public async Task<IActionResult> Update([FromBody] UpdateTemplateSettingCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Update


        #region Delete
        [HttpDelete("Delete")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete([FromBody] DeleteTemplateSettingCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Delete

    }
}
