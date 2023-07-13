using TWJ.TWJApp.TWJService.Api.Extensions;
using TWJ.TWJApp.TWJService.MessageBroker.Services.Server;
using GrpcToolkit.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddServices(builder.Configuration)
                .AddMvc()
                .MvcBuildServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseServices();

app.MapControllers();

app.MapMessageBrokers(typeof(SendDataMiddleware).Assembly);

app.Run();