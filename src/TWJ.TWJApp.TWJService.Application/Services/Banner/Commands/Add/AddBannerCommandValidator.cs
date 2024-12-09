using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.Banner.Commands.Add
{
    public class AddBannerCommandValidator : AbstractValidator<AddBannerCommand>
    {
        private readonly ITWJAppDbContext _context;

        public AddBannerCommandValidator(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Validations();
        }

        private void Validations()
        {
        }
    }
}