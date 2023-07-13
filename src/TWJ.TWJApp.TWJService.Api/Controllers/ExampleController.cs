using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.TestService.Commands.AddItem;
using TWJ.TWJApp.TWJService.Application.Services.TestService.Queries.Get;
using Microsoft.AspNetCore.Mvc;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/examples")]
    public class ExampleController : BaseController
    {
        [HttpPost(":add")]
        public async Task<IActionResult> AddItem([FromBody] TestAddItemCommand command)
        {
            var result = await Mediator.Send(command);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetItems([FromQuery] GetItemsQuery query)
        {
            var result = await Mediator.Send(query);

            return Ok(result);
        }
    }
}
