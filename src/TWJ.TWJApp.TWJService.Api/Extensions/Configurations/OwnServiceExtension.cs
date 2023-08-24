using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.HubNotifier;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.MessageBroker.ChannelConfig.Client;
using TWJ.TWJApp.TWJService.Persistence;

namespace TWJ.TWJApp.TWJService.Api.Extensions.Configurations
{
    public static class OwnServiceExtension
    {
        public static void RegisterOwnService(this IServiceCollection services)
        {
            services.AddScoped<IClient, Client>();
            services.AddSingleton<IHubNotifier, HubNotifier>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ITWJAppDbContext, TWJAppDbContext>();
            services.AddHttpContextAccessor();
        }
    }
}
