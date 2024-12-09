using TWJ.TWJApp.TWJService.Api.HubControllers.Base.Attributes;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace TWJ.TWJApp.TWJService.Api.Extensions.Configurations
{
    public static class SignalRExtension
    {
        public static void RegisterSignalR(this IServiceCollection services)
        {
            services.AddSignalR(options =>
            {
                options.AddFilter<HubFilter>();
                options.EnableDetailedErrors = true;
            }).AddMessagePackProtocol();
        }

        public static void RegisterSignalRControllers<T>(this IEndpointRouteBuilder routeBuilder) where T : Hub
        {
            var basePath = string.Empty;

            var baseAttr = typeof(T).GetCustomAttributes(typeof(RouteHubAttribute), false).FirstOrDefault() as RouteHubAttribute;

            if (baseAttr != null) basePath = baseAttr._path;

            foreach (var type in typeof(T).Assembly.GetTypes().Where(t => typeof(T).IsAssignableFrom(t) && t != typeof(T)))
            {
                if (type.GetCustomAttributes(typeof(RouteHubAttribute), false).Length > 0)
                {
                    var childAttr = type.GetCustomAttributes(typeof(RouteHubAttribute), false).FirstOrDefault() as RouteHubAttribute;

                    string fullPath = childAttr._path.StartsWith("/") || basePath.EndsWith("/") ? $"{basePath}{childAttr._path}" : $"{basePath}/{childAttr._path}";

                    InvokeMapHub(routeBuilder, fullPath, type);
                }
                else
                {
                    var fullPath = type.Name.EndsWith("HubController") ? type.Name.Replace("HubController", string.Empty) : type.Name;
                    InvokeMapHub(routeBuilder, fullPath, type);
                }
            }

        }

        private static void InvokeMapHub(IEndpointRouteBuilder routeBuilder, string fullPath, Type type)
        {
            var invokable = typeof(HubEndpointRouteBuilderExtensions)
                       .GetMethod("MapHub", new Type[] { typeof(IEndpointRouteBuilder), typeof(string) }).MakeGenericMethod(type);

            invokable.Invoke(null, new object[] { routeBuilder, fullPath });
        }
    }
}
