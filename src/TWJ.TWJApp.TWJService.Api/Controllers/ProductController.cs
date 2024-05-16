
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.Base.Queries.GetFiltered;
using TWJ.TWJApp.TWJService.Application.Services.Product.Commands.Add;
using TWJ.TWJApp.TWJService.Application.Services.Product.Commands.Delete;
using TWJ.TWJApp.TWJService.Application.Services.Product.Commands.Import;
using TWJ.TWJApp.TWJService.Application.Services.Product.Commands.Update;
using TWJ.TWJApp.TWJService.Application.Services.Product.Queries.GetAll;
using TWJ.TWJApp.TWJService.Application.Services.Product.Queries.GetFiltered;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseController
    {       
        #region Get-All
        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery]GetAllProductQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Get-All

        #region GetFiltered
        [HttpGet("GetFiltered")]
        public async Task<IActionResult> GetFiltered([FromQuery] GetFilteredProductQuery command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion GetFiltered

        #region Add
        [HttpPost("Add")]
        [AllowAnonymous]
        public async Task<IActionResult> Add([FromBody] AddProductCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Add

        #region Import
        [HttpPost("Import")]
        [AllowAnonymous]
        public async Task<IActionResult> Import(CancellationToken cancellation)
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "clickbank_products.json");
            var result = await Mediator.Send(new ImportProductCommand(filePath));
            return Ok(result);
        }
        #endregion Add

        #region Update
        [HttpPut("Update")]
        [AllowAnonymous]
        public async Task<IActionResult> Update([FromBody] UpdateProductCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Update

        #region Delete
        [HttpDelete("Delete")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete([FromBody] DeleteProductCommand command, CancellationToken cancellation)
        {
            var result = await Mediator.Send(command, cancellation);

            return Ok(result);
        }
        #endregion Delete
    }
}