using TWJ.TWJApp.TWJService.Domain.Enums;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using TWJ.TWJApp.TWJService.Persistence;
using TWJ.TWJApp.TWJService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace TWJ.TWJApp.TWJService.Api.Extensions.Configurations
{
    public static class AuthExtension
    {
        public static void RegisterIdentityServerAuth(this IServiceCollection services, IConfiguration configuration)
        {

            var schema = configuration["Authentication:Schema"];
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = schema;
                options.DefaultChallengeScheme = schema;
            }).AddOAuth2Introspection(options =>
            {
                options.Authority = configuration["Authentication:Authority"];
                options.ClientId = configuration["Authentication:Audience"];
                options.ClientSecret = configuration["Authentication:ApiSecret"];
                options.DiscoveryPolicy.RequireHttps = false;
                options.RoleClaimType = ClaimTypes.Role;
                options.IntrospectionEndpoint = configuration["Authentication:Authority"] + "/connect/introspect";
                options.TokenRetriever = new System.Func<HttpRequest, string>(req =>
                {
                    var fromHeader = TokenRetrieval.FromAuthorizationHeader();
                    var fromQuery = TokenRetrieval.FromQueryString();

                    if (req.Path.StartsWithSegments(HubEnum.BaseUrl)) return fromQuery(req);

                    return fromHeader(req);
                });
            });
        }

        public static void RegisterJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<TWJAppDbContext>()
            .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/hubs")))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization();
        }
    }
}
