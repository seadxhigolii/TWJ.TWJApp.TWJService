using TWJ.TWJApp.TWJService.Common.Helpers;
using TWJ.TWJApp.TWJService.Common.HubNotifier;

namespace TWJ.TWJApp.TWJService.Api.Extensions.Configurations
{
    public static class HelperExtension
    {
        public static void UseRegisteredHelpers(this IApplicationBuilder app)
        {
            AuthHelper.Configure(app.ApplicationServices.GetService<IHttpContextAccessor>());
            HubNotifierHelper.Configure(app.ApplicationServices.GetService<IHubNotifier>());
        }
    }
}
