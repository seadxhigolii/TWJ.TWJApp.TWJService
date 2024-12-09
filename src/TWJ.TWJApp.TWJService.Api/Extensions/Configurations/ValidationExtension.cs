using TWJ.TWJApp.TWJService.Application.Infrastructure.Mediator;
using FluentValidation.AspNetCore;

namespace TWJ.TWJApp.TWJService.Api.Extensions.Configurations
{
    public static class ValidationExtension
    {
        public static void UseValidations(this IMvcBuilder builder)
        {
            builder.AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining(typeof(RequestPerformanceBehavior<,>));
                fv.DisableDataAnnotationsValidation = false;
            });
        }
    }
}
