using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Json;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Helpers.Services;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces.Amazon;
using TWJ.TWJApp.TWJService.Application.Interfaces.Quotes;
using TWJ.TWJApp.TWJService.Application.Interfaces.Video;
using TWJ.TWJApp.TWJService.Application.Services.Amazon.S3;
using TWJ.TWJApp.TWJService.Application.Services.Google;
using TWJ.TWJApp.TWJService.Application.Services.Instagram.Commands.Add;
using TWJ.TWJApp.TWJService.Application.Services.OpenAI;
using TWJ.TWJApp.TWJService.Application.Services.Preplexity;
using TWJ.TWJApp.TWJService.Application.Services.Quotes.BrainyQuotes;
using TWJ.TWJApp.TWJService.Application.Services.Video;
using TWJ.TWJApp.TWJService.Common.HubNotifier;
using TWJ.TWJApp.TWJService.Persistence;

namespace TWJ.TWJApp.TWJService.Api.Extensions.Configurations
{
    public static class OwnServiceExtension
    {
        public static void RegisterOwnService(this IServiceCollection services)
        {
            services.AddHttpClient<OpenAiService>();

            #region Scrappers

            //Beginning of Scrappers
            services.AddTransient<WebScraper.Interfaces.IMedicalXpressScrapperService, WebScraper.Services.MedicalXpressScrapperService>();
            services.AddTransient<WebScraper.Interfaces.IVeryWellHealthScrapperService, WebScraper.Services.VeryWellHealthScrapperService>();
            services.AddTransient<WebScraper.Interfaces.IScienceDailyScrapperService, WebScraper.Services.ScienceDailyScrapperService>();
            services.AddTransient<WebScraper.Interfaces.IMedicalNewsTodayScrapperService, WebScraper.Services.MedicalNewsTodayScrapperService>();

            services.AddTransient<Application.Interfaces.IMedicalXpressService, Application.Services.MedicalXpress.MedicalXpressService>();
            services.AddTransient<Application.Interfaces.IVeryWellHealthScrapperService, Application.Services.VeryWellHealth.VeryWellHealthScrapperService>();
            services.AddTransient<Application.Interfaces.IScienceDailyScrapperService, Application.Services.ScienceDaily.ScienceDailyScrapperService>();
            services.AddTransient<Application.Interfaces.IMedicalNewsTodayScrapperService, Application.Services.MedicalNewsToday.MedicalNewsTodayScrapperService>();
            services.AddTransient<IBrainyQuotesSrapperService, BrainyQuotesScrapperService>();
            //End of Scrappers

            #endregion
            services.AddScoped<AddInstagramPostCommandHandler>();
            services.AddScoped<IOpenAiService, OpenAiService>();
            services.AddScoped<IPreplexityService, PreplexityService>();
            services.AddScoped<IGlobalHelperService, GlobalHelperService>();
            services.AddScoped<IAmazonS3Service, AmazonS3Service>();
            services.AddScoped<IGoogleService, GoogleService>();
            services.AddScoped<IVideoService, VideoService>();
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
