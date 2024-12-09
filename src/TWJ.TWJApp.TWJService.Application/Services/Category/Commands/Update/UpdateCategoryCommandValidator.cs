using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.Category.Commands.Update
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        private readonly ITWJAppDbContext _context;

        public UpdateCategoryCommandValidator(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            Validations();
        }

        private void Validations()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Name")).DependentRules(() =>
            {
                RuleFor(x => x.Name).MustAsync(async (id, cancellation) =>
                {
                    return await _context.ProductCategories.AsNoTracking().AnyAsync(x => x.Name == id, cancellation);
                }).WithMessage(ValidatorMessages.NotFound("Category")).DependentRules(() =>
                {
                    RuleFor(x => x.Name).MustAsync(async (args, id, cancellation) =>
                    {
                        return !await _context.ProductCategories.AsNoTracking().AnyAsync(x => x.Name == args.Name, cancellation);
                    }).WithMessage(x => ValidatorMessages.AlreadyExists($"Category with Property {x.Name}"));
                });
            });

            RuleFor(x => x.Description).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Property"));
        }
    }
}