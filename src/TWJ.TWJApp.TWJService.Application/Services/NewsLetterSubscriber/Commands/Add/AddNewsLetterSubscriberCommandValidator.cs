using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Commands.Add
{
    public class AddNewsLetterSubscriberCommandValidator : AbstractValidator<AddNewsLetterSubscriberCommand>
    {
        private readonly ITWJAppDbContext _context;

        public AddNewsLetterSubscriberCommandValidator(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Validations();
        }

        private void Validations()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Email")).DependentRules(() =>
            {
                RuleFor(x => x.Email).MustAsync(async (email, cancellation) =>
                {
                    return !await _context.NewsLetterSubscribers.AsNoTracking().AnyAsync(x => x.Email.ToLower() == email.ToLower(), cancellation);
                }).WithMessage(x => ValidatorMessages.AlreadyExists($"NewsLetterSubscriber with name {x.Email}"));
            });
        }
    }
}