using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TWJ.TWJApp.TWJService.Api.Controllers.Base
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => (_mediator ??= HttpContext.RequestServices.GetService<IMediator>());
    }
}
