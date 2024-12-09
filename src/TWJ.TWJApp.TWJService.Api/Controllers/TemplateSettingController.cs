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
    [Authorize]

    public class TemplateSettingController : BaseController
    {
        #region Add
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] AddTemplateSettingCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Add

        #region Update
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateTemplateSettingCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Update

        #region Delete
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteTemplateSettingCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Delete

    }
}
