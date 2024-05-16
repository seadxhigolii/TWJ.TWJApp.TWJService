using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPostCategory.Commands.Update
{
    public class UpdateBlogPostCategoryCommandValidator : AbstractValidator<UpdateBlogPostCategoryCommand>
    {
        private readonly ITWJAppDbContext _context;

        public UpdateBlogPostCategoryCommandValidator(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            Validations();
        }

        private void Validations()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Id")).DependentRules(() =>
            {
                RuleFor(x => x.Id).MustAsync(async (id, cancellation) =>
                {
                    return await _context.BlogPostCategories.AsNoTracking().AnyAsync(x => x.Id == id, cancellation);
                }).WithMessage(ValidatorMessages.NotFound("BlogPostCategory")).DependentRules(() =>
                {
                    RuleFor(x => x.Name).MustAsync(async (args, name, cancellation) =>
                    {
                        return !await _context.BlogPostCategories.AsNoTracking().Where(x => x.Name != name).AnyAsync(x => x.Name == args.Name, cancellation);
                    }).WithMessage(x => ValidatorMessages.AlreadyExists($"BlogPostCategory with Property {x.Name}"));
                });
            });

            RuleFor(x => x.Name).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Property"));
            RuleFor(x => x.Description).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Description"));
        }
    }
}