using TWJ.TWJApp.TWJService.Api.Extensions;
using TWJ.TWJApp.TWJService.Persistence;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.IO;
using System.Linq;

namespace TWJ.TWJApp.TWJService.UnitTest.Base
{
    public class UnitContext
    {
        protected IMediator Mediator { get; private set; }
        protected IServiceProvider ServiceProvider { get; private set; }

        public UnitContext()
        {
            ServiceProvider = BuildApp().Services;

            Mediator = ServiceProvider.GetRequiredService<IMediator>();
        }

        private static WebApplication BuildApp()
        {
            var webAppBuilder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                ApplicationName = typeof(Program).Assembly.FullName,
                ContentRootPath = Directory.GetCurrentDirectory(),
                EnvironmentName = "Test",
                WebRootPath = "customwwwroot"
            });

            webAppBuilder.Services.AddServices(webAppBuilder.Configuration)
                                  .AddMvc()
                                  .MvcBuildServices();

            ChangeToMemoryDbContext(webAppBuilder);

            webAppBuilder.Services.AddHttpContextAccessor();

            var builder = webAppBuilder.Build();

            return builder.UseServices();
        }

        private static void ChangeToMemoryDbContext(WebApplicationBuilder builder)
        {
            var context = builder.Services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(TWJAppDbContext));

            if (context != null)
            {
                builder.Services.Remove(context);

                var options = builder
                    .Services
                    .Where(r =>
                          (r.ServiceType == typeof(DbContextOptions)) ||
                          (r.ServiceType.IsGenericType && r.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>)))
                    .ToArray();

                foreach (var option in options)
                {
                    builder.Services.Remove(option);
                }
            }

            builder.Services.AddDbContext<TWJAppDbContext>(opts =>
            {
                opts.UseInMemoryDatabase(databaseName: "Test");
            });
        }
    }
}
