using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Common.HubNotifier
{
    public static class HubNotifierHelper
    {
        private static IHubNotifier _notifier;
        public static void Configure(IHubNotifier notifier)
        {
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        }

        public static async Task Notify<T>() where T : class
        {
            await Task.Factory.StartNew(async () => { await RunInBackground(typeof(T)); });
        }

        private static Task RunInBackground(Type type)
        {
            //if (type == typeof(Group)) await _notifier.GroupsNotify();
            return Task.CompletedTask;
        }
    }
}
