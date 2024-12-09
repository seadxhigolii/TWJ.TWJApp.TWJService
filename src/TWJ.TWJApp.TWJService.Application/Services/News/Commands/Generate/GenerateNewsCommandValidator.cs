using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.News.Commands.Add;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.News.Commands.Generate
{
    public class GenerateNewsCommandValidator : AbstractValidator<AddNewsCommand>
    {
        private readonly ITWJAppDbContext _context;

        public GenerateNewsCommandValidator(ITWJAppDbContext context)
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
