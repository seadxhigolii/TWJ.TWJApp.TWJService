using TWJ.TWJApp.TWJService.Application.Interfaces;
using Microsoft.OpenApi.Models;

namespace TWJ.TWJApp.TWJService.Api.Extensions.Configurations
{
    public static class SwaggerExtension
    {
        public static void AddRegisteredSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = $"{nameof(ITWJAppDbContext)} RestFul API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Provide authentication token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                           {
                             Type = ReferenceType.SecurityScheme,
                             Id = "Bearer"
                           }
                    },
                    Array.Empty<string>()
                    }
                });
            });
        }
        public static void UseRegisteredSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("swagger/v1/swagger.json", "v1");
                options.RoutePrefix = null;
            });
        }
    }
}
