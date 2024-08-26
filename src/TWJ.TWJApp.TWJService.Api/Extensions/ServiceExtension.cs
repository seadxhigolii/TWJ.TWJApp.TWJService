using TWJ.TWJApp.TWJService.Api.Extensions.Configurations;
using TWJ.TWJApp.TWJService.Api.Filters;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using MapperSegregator.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hangfire;
using Hangfire.PostgreSql;
using TWJ.TWJApp.TWJService.Application.Interfaces.Scheduling;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.GenerateRandom;
using TWJ.TWJApp.TWJService.Application.Services.Instagram.Commands.Add;
using MediatR;
using System.Diagnostics;
using TWJ.TWJApp.TWJService.Application.Services.News.Commands.Generate;
using TWJ.TWJApp.TWJService.Application.Services.Email.Commands.Send;

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

            services.AddHangfire(config => config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(opt => opt.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
                    .UseFilter(new AutomaticRetryAttribute { Attempts = 0 }));

            services.AddHangfireServer();

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
            app.UseHangfireDashboard();

            // Ensure UseEndpoints is called after UseRouting
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });

            app.UseRegisteredHelpers();
            app.UseMapperServices();
            ConfigureHangfireJobs(app);
            return app;
        }

        public static void ConfigureHangfireJobs(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var scheduleService = serviceProvider.GetRequiredService<IScheduleService>();
                var mediator = serviceProvider.GetRequiredService<IMediator>();
                var logger = serviceProvider.GetRequiredService<ILogger<ILogger>>();
                var userSettings = scheduleService.GetUserSettingsAsync().Result;
                var cancellationToken = new CancellationToken();

                var timeZoneInfo = TimeZoneInfo.Local;

                var dayMappings = new Dictionary<DayOfWeek, List<string>>
                {
                    { DayOfWeek.Monday, new List<string> { "blog_post_schedule", "instagram_news_monday", "instagram_fact_monday", "instagram_graphic_monday" } },
                    { DayOfWeek.Tuesday, new List<string> { "blog_post_schedule", "instagram_news_tuesday", "instagram_motivationalquote_tuesday" } },
                    { DayOfWeek.Wednesday, new List<string> { "blog_post_schedule", "instagram_news_wednesday", "instagram_fact_wednesday", "instagram_reel_wednesday" } },
                    { DayOfWeek.Thursday, new List<string> { "blog_post_schedule", "instagram_news_thursday", "instagram_healthquote_thursday" } },
                    { DayOfWeek.Friday, new List<string> { "blog_post_schedule", "instagram_news_friday", "instagram_fact_friday", "instagram_graphic_friday" } },
                    { DayOfWeek.Saturday, new List<string> { "blog_post_schedule", "instagram_news_saturday", "instagram_motivationalquote_saturday" } },
                    { DayOfWeek.Sunday, new List<string> { "blog_post_schedule", "instagram_news_sunday", "instagram_reel_sunday" } }
                };

                var expectedJobCount = 0;

                foreach (var dayMapping in dayMappings)
                {
                    expectedJobCount += dayMapping.Value.Count;
                }

                var actualJobCount = 0;

                foreach (var dayMapping in dayMappings)
                {
                    var dayOfWeek = dayMapping.Key;
                    var keys = dayMapping.Value;

                    foreach (var key in keys)
                    {
                        logger.LogInformation($"Processing key: {key} for day: {dayOfWeek}");

                        if (userSettings.TryGetValue(key, out var schedule))
                        {
                            var times = schedule.Split(',');

                            foreach (var time in times)
                            {
                                if (TimeSpan.TryParse(time, out var parsedTime))
                                {
                                    try
                                    {
                                        var cronExpression = Cron.Weekly(dayOfWeek, parsedTime.Hours, parsedTime.Minutes);
                                        var jobId = $"Post-{key}-{dayOfWeek}-{time}";

                                        logger.LogInformation($"Scheduling job {jobId} with cron expression {cronExpression} for key {key} on {dayOfWeek} at {time}");

                                        if (key == "blog_post_schedule")
                                        {
                                            RecurringJob.AddOrUpdate(
                                                jobId,
                                                () => mediator.Send(new GenerateRandomBlogPostCommand(), cancellationToken),
                                                cronExpression,
                                                timeZoneInfo);
                                        }
                                        else
                                        {
                                            var instagramPostCommand = new AddInstagramPostCommand();

                                            switch (key)
                                            {
                                                case string k when k.Contains("instagram_news"):
                                                    instagramPostCommand.Type = 18;
                                                    instagramPostCommand.IsVideo = false;
                                                    break;
                                                case string k when k.Contains("instagram_fact"):
                                                    instagramPostCommand.Type = 20;
                                                    instagramPostCommand.IsVideo = false;
                                                    break;
                                                case string k when k.Contains("instagram_graphic"):
                                                    instagramPostCommand.Type = 21;
                                                    instagramPostCommand.IsVideo = false;
                                                    break;
                                                case string k when k.Contains("instagram_motivationalquote"):
                                                    instagramPostCommand.Type = new Random().Next(1, 18);
                                                    instagramPostCommand.IsVideo = false;
                                                    instagramPostCommand.IsMotivationalQuote = true;
                                                    break;
                                                case string k when k.Contains("instagram_healthquote"):
                                                    instagramPostCommand.Type = new Random().Next(1, 18);
                                                    instagramPostCommand.IsVideo = false;
                                                    break;
                                                case string k when k.Contains("instagram_reel"):
                                                    instagramPostCommand.Type = GetRandomReelType();
                                                    instagramPostCommand.IsVideo = true;
                                                    break;
                                            }

                                            RecurringJob.AddOrUpdate(
                                                jobId,
                                                () => mediator.Send(instagramPostCommand, cancellationToken),
                                                cronExpression,
                                                timeZoneInfo);
                                        }

                                        logger.LogInformation($"Successfully scheduled job {jobId}");
                                        actualJobCount++;
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.Write(ex, $"Error scheduling job {key} at {time} on {dayOfWeek}");
                                    }
                                }
                                else
                                {
                                    logger.LogError($"Failed to parse time '{time}' for key '{key}'");
                                }
                            }
                        }
                        else
                        {
                            logger.LogWarning($"No schedule found for key '{key}'");
                        }
                    }
                }

                // Schedule Type 1 News Generation at 1 AM Local Time
                var type1NewsJobId = "GenerateNews-Type1-1AM";
                var type1CronExpression = Cron.Daily(1, 0); // 1 AM
                RecurringJob.AddOrUpdate(
                    type1NewsJobId,
                    () => mediator.Send(new GenerateNewsCommand { NewsType = 1 }, new CancellationToken()),
                    type1CronExpression,
                    timeZoneInfo);
                logger.LogInformation($"Successfully scheduled Type 1 news generation job at 1 AM");

                // Schedule Type 2 News Generation at 2 AM Local Time
                var type2NewsJobId = "GenerateNews-Type2-2AM";
                var type2CronExpression = Cron.Daily(2, 0); // 2 AM
                RecurringJob.AddOrUpdate(
                    type2NewsJobId,
                    () => mediator.Send(new GenerateNewsCommand { NewsType = 2 }, new CancellationToken()),
                    type2CronExpression,
                    timeZoneInfo);
                logger.LogInformation($"Successfully scheduled Type 2 news generation job at 2 AM");

                // Schedule Type 3 News Generation at 3 AM Local Time
                var type3NewsJobId = "GenerateNews-Type3-3AM";
                var type3CronExpression = Cron.Daily(3, 0); // 3 AM
                RecurringJob.AddOrUpdate(
                    type3NewsJobId,
                    () => mediator.Send(new GenerateNewsCommand { NewsType = 3 }, new CancellationToken()),
                    type3CronExpression,
                    timeZoneInfo);
                logger.LogInformation($"Successfully scheduled Type 3 news generation job at 3 AM");

                // Schedule Type 4 News Generation at 4 AM Local Time
                var type4NewsJobId = "GenerateNews-Type4-4AM";
                var type4CronExpression = Cron.Daily(4, 0); // 4 AM
                RecurringJob.AddOrUpdate(
                    type4NewsJobId,
                    () => mediator.Send(new GenerateNewsCommand { NewsType = 4 }, new CancellationToken()),
                    type4CronExpression,
                    timeZoneInfo);
                logger.LogInformation($"Successfully scheduled Type 4 news generation job at 4 AM");

                logger.LogInformation("News generation jobs successfully scheduled for Types 1-4 at 1 AM to 4 AM");

                var emailJobId = "SendEmailCommand-Saturday-11AM";
                var emailCronExpression = Cron.Weekly(DayOfWeek.Saturday, 11, 0);

                RecurringJob.AddOrUpdate(
                    emailJobId,
                    () => mediator.Send(new SendEmailCommand(), cancellationToken),
                    emailCronExpression,
                    timeZoneInfo);

                logger.LogInformation($"Successfully scheduled email job {emailJobId}");

                logger.LogInformation($"Expected job count: {expectedJobCount}, Actual job count: {actualJobCount}");
            }
        }

        private static int GetRandomReelType()
        {
            var random = new Random();
            var type = random.Next(1, 16);
            return type == 14 ? 15 : type;
        }


    }
}
