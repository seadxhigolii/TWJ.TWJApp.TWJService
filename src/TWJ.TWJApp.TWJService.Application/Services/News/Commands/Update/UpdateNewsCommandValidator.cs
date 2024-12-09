using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.News.Commands.Update
{
    public class UpdateNewsCommandValidator : AbstractValidator<UpdateNewsCommand>
    {
        private readonly ITWJAppDbContext _context;

        public UpdateNewsCommandValidator(ITWJAppDbContext context)
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
                    return await _context.News.AsNoTracking().AnyAsync(x => x.Id == id, cancellation);
                }).WithMessage(ValidatorMessages.NotFound("News")).DependentRules(() =>
                {
                    RuleFor(x => x.Title).MustAsync(async (args, id, cancellation) =>
                    {
                        return !await _context.News.AsNoTracking().Where(x => x.Title != id).AnyAsync(x => x.Title == args.Title, cancellation);
                    }).WithMessage(x => ValidatorMessages.AlreadyExists($"News with Property {x.Title}"));
                });
            });

            RuleFor(x => x.Title).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Property"));
        }
    }
}