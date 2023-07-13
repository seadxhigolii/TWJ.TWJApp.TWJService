using TWJ.TWJApp.TWJService.Api.HubControllers;
using TWJ.TWJApp.TWJService.Common.HubNotifier;
using Microsoft.AspNetCore.SignalR;

namespace TWJ.TWJApp.TWJService.Application.Interfaces
{
    public class HubNotifier : IHubNotifier
    {
        private readonly IHubContext<TestHubController> _testHubContext;

        public HubNotifier(IHubContext<TestHubController> calendarHubContext)
        {
            _testHubContext = calendarHubContext ?? throw new ArgumentNullException(nameof(calendarHubContext));
        }

        public async Task CalendarNotify()
        {
            await _testHubContext.Clients.All.SendAsync("GetCalendarEventsResponse");
        }
    }
}
