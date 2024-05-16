using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.Product.Commands.Update
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        private readonly ITWJAppDbContext _context;

        public UpdateProductCommandValidator(ITWJAppDbContext context)
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
                    return await _context.Products.AsNoTracking().AnyAsync(x => x.Id == id, cancellation);
                }).WithMessage(ValidatorMessages.NotFound("Product")).DependentRules(() =>
                {
                    RuleFor(x => x.Id).MustAsync(async (args, id, cancellation) =>
                    {
                        return !await _context.Products.AsNoTracking().Where(x => x.Id != id).AnyAsync(x => x.ProductName == args.ProductName, cancellation);
                    }).WithMessage(x => ValidatorMessages.AlreadyExists($"Product with Property {x.ProductName}"));
                });
            });
        }
    }
}