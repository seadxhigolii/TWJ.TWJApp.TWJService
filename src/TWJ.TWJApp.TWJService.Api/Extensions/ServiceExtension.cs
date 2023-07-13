using TWJ.TWJApp.TWJService.Api.Extensions.Configurations;
using TWJ.TWJApp.TWJService.Api.Filters;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using GrpcToolkit.Extensions;
using MapperSegregator.Extensions.DependencyInjection;

namespace TWJ.TWJApp.TWJService.Api.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterCors();
            services.RegisterDbContext(configuration);
            //services.RegisterIdentityServerAuth(configuration);
            //services.RegisterJwtAuth(configuration);
            services.AddRegisteredSwagger();
            services.RegisterMediator();
            services.AddGrpc();
            services.RegisterOwnService();
            services.RegisterSignalR();
            services.RegisterGrpcToolkit();
            services.RegisterMapperServices(typeof(ITWJAppDbContext).Assembly);

            return services;
        }

        public static IMvcBuilder MvcBuildServices(this IMvcBuilder builder)
        {
            builder.AddMvcOptions(options =>
            {
                options.Filters.Add<ExceptionFilter>();
            });
            builder.AddNewtonsoftJson();
            builder.UseValidations();

            return builder;
        }
        public static WebApplication UseServices(this WebApplication app)
        {
            app.UseRegisteredCors();
            app.UseHttpsRedirection();
            app.UseRegisteredSwagger();
            app.UseAuthentication();
            //app.UseAuthorization();
            app.UseRegisteredHelpers();
            app.UseMapperServices();
            return app;
        }
    }
}
