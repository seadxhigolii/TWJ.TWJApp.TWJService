using TWJ.TWJApp.TWJService.Api.Extensions.Configurations;
using TWJ.TWJApp.TWJService.Api.Filters;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using GrpcToolkit.Extensions;
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
            services.AddGrpc();
            services.RegisterOwnService();
            services.RegisterSignalR();
            services.RegisterGrpcToolkit();
            services.RegisterMapperServices(typeof(ITWJAppDbContext).Assembly);
            var key = Encoding.ASCII.GetBytes("G%__Q8f(r%.c|up_?~PloAxVi^Tb`H*@Nt}w0Rt*f([r;`[,mT/Ks**-dIt~0tx");
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
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRegisteredHelpers();
            app.UseMapperServices();
            return app;
        }
    }
}
