namespace TWJ.TWJApp.TWJService.Api.Extensions.Configurations
{
    public static class CorsExtension
    {
        public const string corsPolicyName = "SiteCorsPolicy";
        public static void RegisterCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                    .WithOrigins("http://localhost:8080")
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

        }

        public static void UseRegisteredCors(this IApplicationBuilder app)
        {
            app.UseCors();
        }
    }

}
