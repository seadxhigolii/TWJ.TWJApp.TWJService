using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.Base.Queries.GetFiltered;
using TWJ.TWJApp.TWJService.Application.Services.Template.Commands.Add;
using TWJ.TWJApp.TWJService.Application.Services.Template.Commands.Delete;
using TWJ.TWJApp.TWJService.Application.Services.Template.Commands.Update;
using TWJ.TWJApp.TWJService.Application.Services.Template.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class TemplateController : BaseController
    {

        #region Get-All
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery]GetAllTemplatesQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Get-All

        #region GetFiltered
        [HttpGet("GetFiltered")]
        public async Task<IActionResult> GetFiltered([FromQuery] GetFilteredTemplateQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion GetFiltered

        #region Add
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] AddTemplateCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Add

        #region Update
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateTemplateCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Update

        #region Delete
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteTemplateCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Delete

    }
}
