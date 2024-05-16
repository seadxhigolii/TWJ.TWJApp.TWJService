using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.News.Commands.Add
{
    public class AddNewsCommandValidator : AbstractValidator<AddNewsCommand>
    {
        private readonly ITWJAppDbContext _context;

        public AddNewsCommandValidator(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Validations();
        }

        private void Validations()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(ValidatorMessages.NotEmpty("Title"))
                .MustAsync(async (title, cancellation) =>
                {
                    return !await _context.News
                        .AsNoTracking()
                        .AnyAsync(x => x.Title.ToLower() == title.ToLower(), cancellation);
                })
                .WithMessage(x => ValidatorMessages.AlreadyExists($"News with name {x.Title}"));
        }

    }
}