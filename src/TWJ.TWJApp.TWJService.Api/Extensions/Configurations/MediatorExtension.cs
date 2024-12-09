using TWJ.TWJApp.TWJService.Application.Infrastructure.Mediator;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using MediatR;

namespace TWJ.TWJApp.TWJService.Api.Extensions.Configurations
{
    public static class MediatorExtension
    {
        public static void RegisterMediator(this IServiceCollection services)
        {
            services.AddMediatR(typeof(ITWJAppDbContext).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehavior<,>));
        }
    }
}
