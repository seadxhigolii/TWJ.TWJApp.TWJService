using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.Category.Commands.Add
{
    public class AddCategoryCommandValidator : AbstractValidator<AddCategoryCommand>
    {
        private readonly ITWJAppDbContext _context;

        public AddCategoryCommandValidator(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Validations();
        }

        private void Validations()
        {
            //RuleFor(x => x.Name).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Property")).DependentRules(() =>
            //{
            //    RuleFor(x => x.Name).MustAsync(async (name, cancellation) =>
            //    {
            //        return !await _context.ProductCategories.AsNoTracking().AnyAsync(x => x.Name.ToLower() == name.ToLower(), cancellation);
            //    }).WithMessage(x => ValidatorMessages.AlreadyExists($"Category with name {x.Name}"));
            //});
        }
    }
}