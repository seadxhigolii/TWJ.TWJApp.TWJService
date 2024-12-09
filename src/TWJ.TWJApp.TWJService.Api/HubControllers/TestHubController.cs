using TWJ.TWJApp.TWJService.Api.HubControllers.Base;
using TWJ.TWJApp.TWJService.Api.HubControllers.Base.Attributes;

namespace TWJ.TWJApp.TWJService.Api.HubControllers
{
    [RouteHub("/calendar/events")]
    public class TestHubController : BaseHub
    {
        public Task GetCalendarEvents()
        {
            //var result = await Authorize(this).StartSafe(async () => await Mediator.Send(query));

            //await Clients.Caller.SendAsync(ResponseHub(), result);

            return Task.CompletedTask;
        }
    }
}
