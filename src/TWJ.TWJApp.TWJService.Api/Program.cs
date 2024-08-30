using Hangfire;
using MapperSegregator.Extensions.DependencyInjection;
using TWJ.TWJApp.TWJService.Api.Extensions.Configurations;
using TWJ.TWJApp.TWJService.Api.Extensions;
using TWJ.TWJApp.TWJService.Application.Services.OpenAI;
using TWJ.TWJApp.TWJService.Api.Middlewares;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200", "https://www.twj-health.com", "https://twj-health.com")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});

builder.Services.AddHttpClient<OpenAiService>();

builder.Services.AddServices(builder.Configuration)
                .AddMvc()
                .MvcBuildServices();

// Add Swagger only in non-production environments
if (!builder.Environment.IsProduction())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

app.UseRouting();

//app.UseMiddleware<AnonymousRateLimitingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

if (!builder.Environment.IsProduction())
{
    app.UseHangfireDashboard();
}

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

ServiceExtension.ConfigureHangfireJobs(app);

app.Run();
