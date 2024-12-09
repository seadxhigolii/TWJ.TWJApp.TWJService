
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.Add
{
    public class AddBlogPostCommandValidator : AbstractValidator<AddBlogPostCommand>
    {
        private readonly ITWJAppDbContext _context;

        public AddBlogPostCommandValidator(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Validations();
        }

        private void Validations()
        {
            RuleFor(x => x.Title)
              .NotEmpty().WithMessage("Title is required.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.");
        }
    }
}