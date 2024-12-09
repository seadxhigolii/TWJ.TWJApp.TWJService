using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.Product.Commands.Add
{
    public class AddProductCommandValidator : AbstractValidator<AddProductCommand>
    {
        private readonly ITWJAppDbContext _context;

        public AddProductCommandValidator(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Validations();
        }

        private void Validations()
        {
            //RuleFor(x => x.ProductName).NotEmpty().WithMessage(ValidatorMessages.NotEmpty("ProductName")).DependentRules(() =>
            //{
            //    RuleFor(x => x.ProductName).MustAsync(async (name, cancellation) =>
            //    {
            //        return !await _context.Products.AsNoTracking().AnyAsync(x => x.ProductName.ToLower() == name.ToLower(), cancellation);
            //    }).WithMessage(x => ValidatorMessages.AlreadyExists($"Product with name {x.ProductName}"));
            //});
        }
    }
}