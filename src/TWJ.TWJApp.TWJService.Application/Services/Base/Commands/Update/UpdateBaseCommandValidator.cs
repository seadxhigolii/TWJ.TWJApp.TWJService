using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Commands.Update
{
    public class UpdateBaseCommandValidator : AbstractValidator<UpdateBaseCommand>
    {
        private readonly ITWJAppDbContext _context;
        public UpdateBaseCommandValidator(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            Validations();
        }

        private void Validations()
        {
            RuleFor(x => x.Property).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Id")).DependentRules(() =>
            {
                RuleFor(x => x.Property).MustAsync(async (id, cancellation) =>
                {
                    return await _context.Base.AsNoTracking().AnyAsync(x => x.Property == id, cancellation);
                }).WithMessage(ValidatorMessages.NotFound("Base")).DependentRules(() =>
                {
                    RuleFor(x => x.Property).MustAsync(async (args, id, cancellation) =>
                    {
                        return !await _context.Base.AsNoTracking().Where(x => x.Property != id).AnyAsync(x => x.Property == args.Property, cancellation);
                    }).WithMessage(x => ValidatorMessages.AlreadyExists($"Base with Property {x.Property}"));
                });
            });

            RuleFor(x => x.Property).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("Property"));
        }
    }
}
