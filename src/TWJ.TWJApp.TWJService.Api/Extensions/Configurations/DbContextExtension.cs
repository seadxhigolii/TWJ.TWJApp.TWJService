using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TWJ.TWJApp.TWJService.Api.Extensions.Configurations
{
    public static class DbContextExtension
    {
        public static void RegisterDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITWJAppDbContext, TWJAppDbContext>();

            services.AddDbContext<TWJAppDbContext>(x => x.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), option =>
            {
                option.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            }));
        }
    }
}
