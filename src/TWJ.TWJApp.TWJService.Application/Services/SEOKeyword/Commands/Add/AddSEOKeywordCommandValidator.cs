using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Commands.Add
{
    public class AddSEOKeywordCommandValidator : AbstractValidator<AddSEOKeywordCommand>
    {
        private readonly ITWJAppDbContext _context;

        public AddSEOKeywordCommandValidator(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Validations();
        }

        private void Validations()
        {
            RuleFor(x => x.Keyword).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Keyword")).DependentRules(() =>
            {
                RuleFor(x => x.Keyword).MustAsync(async (name, cancellation) =>
                {
                    return !await _context.SEOKeywords.AsNoTracking().AnyAsync(x => x.Keyword.ToLower() == name.ToLower(), cancellation);
                }).WithMessage(x => ValidatorMessages.AlreadyExists($"Data with name {x.Keyword}"));
            });
        }
    }
}