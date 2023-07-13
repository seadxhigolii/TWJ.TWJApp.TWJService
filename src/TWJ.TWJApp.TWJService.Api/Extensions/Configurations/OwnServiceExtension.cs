using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.HubNotifier;
using TWJ.TWJApp.TWJService.MessageBroker.ChannelConfig.Client;

namespace TWJ.TWJApp.TWJService.Api.Extensions.Configurations
{
    public static class OwnServiceExtension
    {
        public static void RegisterOwnService(this IServiceCollection services)
        {
            services.AddScoped<IClient, Client>();
            services.AddSingleton<IHubNotifier, HubNotifier>();
        }
    }
}
