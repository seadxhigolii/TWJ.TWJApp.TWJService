using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.Instagram.Commands.Add;
using TWJ.TWJApp.TWJService.Application.Services.Instagram.Commands.AddTemplate;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InstagramController : BaseController
    {       

        #region Add
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] AddInstagramPostCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Add       

        #region AddTemplate
        [HttpPost("AddTemplate")]
        public async Task<IActionResult> AddTemplate([FromBody] AddInstagramTemplateCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Add   
    }
}