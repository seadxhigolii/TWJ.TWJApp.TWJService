using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Update
{
    public class UpdateTemplateSettingCommandValidator : AbstractValidator<UpdateTemplateSettingCommand>
    {
        private readonly ITWJAppDbContext _context;
        public UpdateTemplateSettingCommandValidator(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            Validations();
        }

        private void Validations()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Id")).DependentRules(() =>
            {
                RuleFor(x => x.Name).MustAsync(async (id, cancellation) =>
                {
                    return await _context.TemplateSetting.AsNoTracking().AnyAsync(x => x.Name == id, cancellation);
                }).WithMessage(ValidatorMessages.NotFound("TemplateSetting")).DependentRules(() =>
                {
                    RuleFor(x => x.Name).MustAsync(async (args, id, cancellation) =>
                    {
                        return !await _context.TemplateSetting.AsNoTracking().Where(x => x.Name != id).AnyAsync(x => x.Name == args.Name, cancellation);
                    }).WithMessage(x => ValidatorMessages.AlreadyExists($"Template Setting with Name {x.Name}"));
                });
            });

            RuleFor(x => x.Name).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Name"));
        }
    }
}
