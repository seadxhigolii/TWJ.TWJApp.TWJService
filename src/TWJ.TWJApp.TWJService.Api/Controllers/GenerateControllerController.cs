using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerateControllerController : BaseController
    {
        [HttpPost("GenerateController/{entityName}")]
        [AllowAnonymous]
        public async Task<IActionResult> Generate(string entityName)
        {
            await GenerateControllerForEntity(entityName);
            return Ok($"Controller for {entityName} generated successfully.");
        }

        private async Task GenerateControllerForEntity(string entityName)
        {

            var rootDirectory = @"C:\Sources\thewellnessjunction\src\TWJ.TWJApp.TWJService.Api\Controllers";


            var controllerContent = $@"
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.{entityName}.Commands.Add;
using TWJ.TWJApp.TWJService.Application.Services.{entityName}.Commands.Delete;
using TWJ.TWJApp.TWJService.Application.Services.{entityName}.Commands.Update;
using TWJ.TWJApp.TWJService.Application.Services.{entityName}.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{{
    [Route(""api/[controller]"")]
    [ApiController]
    public class {entityName}Controller : BaseController
    {{       
        #region Get-All
        [HttpGet(""GetAll"")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery]GetAll{entityName}Query command, CancellationToken cancellation)
        {{
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }}
        #endregion Get-All

        #region Add
        [HttpPost(""Add"")]
        [AllowAnonymous]
        public async Task<IActionResult> Add([FromBody] Add{entityName}Command command, CancellationToken cancellation)
        {{
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }}
        #endregion Add

        #region Update
        [HttpPut(""Update"")]
        [AllowAnonymous]
        public async Task<IActionResult> Update([FromBody] Update{entityName}Command command, CancellationToken cancellation)
        {{
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }}
        #endregion Update

        #region Delete
        [HttpDelete(""Delete"")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete([FromBody] Delete{entityName}Command command, CancellationToken cancellation)
        {{
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }}
        #endregion Delete
    }}
}}";

            var filePath = Path.Combine(rootDirectory, $"{entityName}Controller.cs");
            await System.IO.File.WriteAllTextAsync(filePath, controllerContent);
        }
    }
}
