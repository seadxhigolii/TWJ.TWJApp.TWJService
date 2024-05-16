using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Commands.Update
{
    public class UpdateSEOKeywordCommandValidator : AbstractValidator<UpdateSEOKeywordCommand>
    {
        private readonly ITWJAppDbContext _context;

        public UpdateSEOKeywordCommandValidator(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            Validations();
        }

        private void Validations()
        {
            RuleFor(x => x.Keyword).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Id")).DependentRules(() =>
            {
                RuleFor(x => x.Keyword).MustAsync(async (id, cancellation) =>
                {
                    return await _context.SEOKeywords.AsNoTracking().AnyAsync(x => x.Keyword == id, cancellation);
                }).WithMessage(ValidatorMessages.NotFound("SEOKeyword")).DependentRules(() =>
                {
                    RuleFor(x => x.Keyword).MustAsync(async (args, id, cancellation) =>
                    {
                        return !await _context.SEOKeywords.AsNoTracking().Where(x => x.Keyword != id).AnyAsync(x => x.Keyword == args.Keyword, cancellation);
                    }).WithMessage(x => ValidatorMessages.AlreadyExists($"SEOKeyword with Property {x.Keyword}"));
                });
            });

            RuleFor(x => x.Keyword).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Property"));
        }
    }
}