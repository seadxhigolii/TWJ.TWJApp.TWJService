
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.Product.Commands.Import;
using TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Commands.Add;
using TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Commands.Delete;
using TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Commands.Import;
using TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Commands.Update;
using TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SEOKeywordController : BaseController
    {       
        #region Get-All
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery]GetAllSEOKeywordQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Get-All

        #region Add
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] AddSEOKeywordCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Add

        #region Import
        [HttpPost("Import")]
        public async Task<IActionResult> Import(CancellationToken cancellation)
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "seokeywords.csv");
            var result = await Mediator.Send(new ImportSEOKeywordCommand(filePath));
            return Ok(result);
        }
        #endregion Import

        #region Update
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateSEOKeywordCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Update

        #region Delete
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteSEOKeywordCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Delete
    }
}