using Hangfire;
using MapperSegregator.Extensions.DependencyInjection;
using TWJ.TWJApp.TWJService.Api.Extensions.Configurations;
using TWJ.TWJApp.TWJService.Api.Extensions;
using TWJ.TWJApp.TWJService.Application.Services.OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on specific URLs for both HTTP and HTTPS
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000); // HTTP port
    serverOptions.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.UseHttps(); // HTTPS port
    });
});


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

// Add HTTP client services
builder.Services.AddHttpClient<OpenAiService>();

// Add custom services and MVC configuration
builder.Services.AddServices(builder.Configuration)
                .AddMvc()
                .MvcBuildServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseRouting();

// app.UseMiddleware<AnonymousRateLimitingMiddleware>();

// Configure middleware based on environment
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

// Handle HTTPS redirection (if needed)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();  // Optionally skip this if Nginx is handling HTTPS
}

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard();

// Set up endpoint routing
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllers();
    endpoints.MapHangfireDashboard();  // If using Hangfire dashboard
});

// Custom service registrations and helpers
app.UseRegisteredHelpers();
app.UseMapperServices();

// Configure Hangfire jobs (if using Hangfire)
ServiceExtension.ConfigureHangfireJobs(app);

// Run the application
app.Run();