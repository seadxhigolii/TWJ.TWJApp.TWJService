using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Add;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.Template.Commands.Add
{
    public class AddTemplateCommandValidator : AbstractValidator<AddTemplateCommand>
    {
        private readonly ITWJAppDbContext _context;
        public AddTemplateCommandValidator(ITWJAppDbContext context)
        {

            _context = context ?? throw new ArgumentNullException(nameof(context));
            Validations();
        }

        private void Validations()
        {
            RuleFor(x => x.DisplayText)
                .NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Property"))
                .DependentRules(() =>
                {
                    RuleFor(x => x.DisplayText).MustAsync(async (name, cancellation) =>
                    {
                        bool exists = await _context.Templates.AsNoTracking().AnyAsync(x => x.DisplayText == name, cancellation);
                        return !exists;
                    }).WithMessage(x => ValidatorMessages.AlreadyExists($"Template with name {x.DisplayText}"));
                });

            RuleFor(x => x.IsDefault)
                .MustAsync(async (template, isDefault, cancellation) =>
                {
                    if (isDefault)
                    {
                        bool isDefaultAlreadyExists = await _context.Templates.AsNoTracking().AnyAsync(t => t.IsDefault && t.TemplateSettingId == template.TemplateSettingId, cancellation);
                        return !isDefaultAlreadyExists;
                    }
                    return true;
                }).WithMessage(x => ValidatorMessages.DefaultExists($"{x.DisplayText}"));

            RuleFor(x => x.ParentId)
                 .MustAsync(async (parentId, cancellation) =>
                 {
                     if (parentId.HasValue)  // Check if ParentId is not null
                     {
                         bool parentIdExists = await _context.Templates.AsNoTracking().AnyAsync(t => t.Id == parentId.Value, cancellation);
                         return parentIdExists;
                     }
                     return true;
                 }).WithMessage(x => ValidatorMessages.NotFound("This Parent"));
        }


    }
}
