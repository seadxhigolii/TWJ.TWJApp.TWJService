using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPostCategory.Commands.Add
{
    public class AddBlogPostCategoryCommandValidator : AbstractValidator<AddBlogPostCategoryCommand>
    {
        private readonly ITWJAppDbContext _context;

        public AddBlogPostCategoryCommandValidator(ITWJAppDbContext context)
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
                    return !await _context.BlogPostCategories.AsNoTracking().AnyAsync(x => x.Name.ToLower() == name.ToLower(), cancellation);
                }).WithMessage(x => ValidatorMessages.AlreadyExists($"BlogPostCategory with name {x.Name}"));
            });
        }
    }
}