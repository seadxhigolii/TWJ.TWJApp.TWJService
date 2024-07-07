
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Commands.Update
{
    public class UpdateBlogPostCommandValidator : AbstractValidator<UpdateBlogPostCommand>
    {
        private readonly ITWJAppDbContext _context;

        public UpdateBlogPostCommandValidator(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            Validations();
        }

        private void Validations()
        {
            RuleFor(x => x.Title)
              .NotEmpty().WithMessage("Title is required.")
              .MinimumLength(5).WithMessage("Title should be at least 5 characters.")
              .MaximumLength(100).WithMessage("Title should be at most 100 characters.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.");
        }
    }
}