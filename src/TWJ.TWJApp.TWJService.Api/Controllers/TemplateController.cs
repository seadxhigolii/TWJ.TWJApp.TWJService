using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.Account.Commands.Login;
using TWJ.TWJApp.TWJService.Application.Services.Account.Commands.Register;
using TWJ.TWJApp.TWJService.Application.Services.Template.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TemplateController : BaseController
    {
        #region Get-All
        [HttpPost("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromBody] GetAllTemplatesQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Get-All

    }
}
