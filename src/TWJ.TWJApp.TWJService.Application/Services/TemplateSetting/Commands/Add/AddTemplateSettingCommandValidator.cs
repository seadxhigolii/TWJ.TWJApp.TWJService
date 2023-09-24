using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Add
{
    public class AddTemplateSettingCommandValidator : AbstractValidator<AddTemplateSettingCommand>
    {
        private readonly ITWJAppDbContext _context;
        public AddTemplateSettingCommandValidator(ITWJAppDbContext context)
        {

            _context = context ?? throw new ArgumentNullException(nameof(context));
            Validations();
        }

        private void Validations()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Name")).DependentRules(() =>
            {
                RuleFor(x => x.Name).MustAsync(async (name, cancellation) =>
                {
                    return !await _context.TemplateSetting.AsNoTracking().AnyAsync(x => x.Name.ToLower() == name.ToLower(), cancellation);
                }).WithMessage(x => ValidatorMessages.AlreadyExists($"Template Setting with name {x.Name}"));
            });
        }
    }
}
