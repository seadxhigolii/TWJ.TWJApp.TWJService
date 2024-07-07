using TWJ.TWJApp.TWJService.Api.Extensions.Configurations;
using TWJ.TWJApp.TWJService.Api.Filters;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using MapperSegregator.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.RegisterOwnService();
            services.RegisterSignalR();
            services.RegisterMapperServices(typeof(ITWJAppDbContext).Assembly);
            var key = Encoding.ASCII.GetBytes(configuration["AppSettings:Token"]);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
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

            // Add UseRouting here
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Ensure UseEndpoints is called after UseRouting
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllers();
            });

            app.UseRegisteredHelpers();
            app.UseMapperServices();
            return app;
        }
    }
}
