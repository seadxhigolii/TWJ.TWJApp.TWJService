using TWJ.TWJApp.TWJService.Api.HubControllers.Base.Attributes;
using TWJ.TWJApp.TWJService.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace TWJ.TWJApp.TWJService.Api.HubControllers.Base
{
    [RouteHub(HubEnum.BaseUrl)]
    public class BaseHub : Hub
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= Context.GetHttpContext().RequestServices.GetService<IMediator>();
    }
}
