using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Add
{
    public class AddBaseCommandValidator : AbstractValidator<AddBaseCommand>
    {
        private readonly ITWJAppDbContext _context;
        public AddBaseCommandValidator(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Validations();
        }

        private void Validations()
        {
            RuleFor(x => x.Property).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Property")).DependentRules(() =>
            {
                RuleFor(x => x.Property).MustAsync(async (name, cancellation) =>
                {
                    return !await _context.Base.AsNoTracking().AnyAsync(x => x.Property.ToLower() == name.ToLower(), cancellation);
                }).WithMessage(x => ValidatorMessages.AlreadyExists($"Art with name {x.Property}"));
            });
        }
    }
}
