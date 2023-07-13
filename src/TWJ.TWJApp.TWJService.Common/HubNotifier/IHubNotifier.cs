using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Common.HubNotifier
{
    public interface IHubNotifier
    {
        Task CalendarNotify();
    }
}
