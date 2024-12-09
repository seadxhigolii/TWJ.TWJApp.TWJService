using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Commands.Update
{
    public class UpdateNewsLetterSubscriberCommandValidator : AbstractValidator<UpdateNewsLetterSubscriberCommand>
    {
        private readonly ITWJAppDbContext _context;

        public UpdateNewsLetterSubscriberCommandValidator(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            Validations();
        }

        private void Validations()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Id")).DependentRules(() =>
            {
                RuleFor(x => x.Email).MustAsync(async (id, cancellation) =>
                {
                    return await _context.NewsLetterSubscribers.AsNoTracking().AnyAsync(x => x.Email == id, cancellation);
                }).WithMessage(ValidatorMessages.NotFound("NewsLetterSubscriber")).DependentRules(() =>
                {
                    RuleFor(x => x.Email).MustAsync(async (args, id, cancellation) =>
                    {
                        return !await _context.NewsLetterSubscribers.AsNoTracking().Where(x => x.Email != id).AnyAsync(x => x.Email == args.Email, cancellation);
                    }).WithMessage(x => ValidatorMessages.AlreadyExists($"NewsLetterSubscriber with Property {x.Email}"));
                });
            });

            RuleFor(x => x.Email).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Email"));
        }
    }
}