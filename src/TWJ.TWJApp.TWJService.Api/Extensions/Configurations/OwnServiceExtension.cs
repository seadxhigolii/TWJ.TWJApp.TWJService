using Amazon.S3;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Json;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Helpers.Services;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces.Amazon;
using TWJ.TWJApp.TWJService.Application.Services.Amazon.S3;
using TWJ.TWJApp.TWJService.Application.Services.Google;
using TWJ.TWJApp.TWJService.Application.Services.MedicalXpress;
using TWJ.TWJApp.TWJService.Application.Services.OpenAI;
using TWJ.TWJApp.TWJService.Application.Services.Preplexity;
using TWJ.TWJApp.TWJService.Common.HubNotifier;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Persistence;
using WebScraper.Interfaces;
using WebScraper.Services;

namespace TWJ.TWJApp.TWJService.Api.Extensions.Configurations
{
    public static class OwnServiceExtension
    {
        public static void RegisterOwnService(this IServiceCollection services)
        {
            services.AddHttpClient<OpenAiService>();
            services.AddTransient<IMedicalXpressScrapperService, MedicalXpressScrapperService>();
            services.AddTransient<IMedicalXpressService, MedicalXpressService>();
            services.AddScoped<IOpenAiService, OpenAiService>();
            services.AddScoped<IPreplexityService, PreplexityService>();
            services.AddScoped<IGlobalHelperService, GlobalHelperService>();
            services.AddScoped<IAmazonS3Service, AmazonS3Service>();
            services.AddScoped<IGoogleService, GoogleService>();
            services.AddScoped<ITWJAppDbContext, TWJAppDbContext>();
            services.AddSingleton<IHubNotifier, HubNotifier>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpContextAccessor();
            services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
        }
    }
}
