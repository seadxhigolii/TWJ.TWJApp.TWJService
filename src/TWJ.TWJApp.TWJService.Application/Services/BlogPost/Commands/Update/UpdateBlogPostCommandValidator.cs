
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

            RuleFor(x => x.CategoryId)
                .NotEqual(default(Guid)).WithMessage("CategoryId is required.");

            RuleFor(x => x.Tags)
                .MaximumLength(500).WithMessage("Tags should be at most 500 characters.");

            RuleFor(x => x.Image)
                .Must(BeAValidSize).WithMessage("Image must be less than 5 MB.")
                .When(x => x.Image != null);

            RuleFor(x => x.Views)
                .GreaterThanOrEqualTo(0).WithMessage("Views cannot be negative.");

            RuleFor(x => x.Likes)
                .GreaterThanOrEqualTo(0).WithMessage("Likes cannot be negative.");

            RuleFor(x => x.Dislikes)
                .GreaterThanOrEqualTo(0).WithMessage("Dislikes cannot be negative.");

            RuleFor(x => x.NumberOfComments)
                .GreaterThanOrEqualTo(0).WithMessage("NumberOfComments cannot be negative.");

            RuleFor(x => x.ProductID)
                .NotEqual(default(Guid?)).When(x => x.ProductID.HasValue);
        }
        private bool BeAValidSize(byte[] image)
        {
            return image.Length <= 5 * 1024 * 1024;
        }
    }
}