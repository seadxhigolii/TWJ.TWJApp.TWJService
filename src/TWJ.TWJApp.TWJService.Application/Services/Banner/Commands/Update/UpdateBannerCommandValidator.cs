using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.Banner.Commands.Update
{
    public class UpdateBannerCommandValidator : AbstractValidator<UpdateBannerCommand>
    {
        private readonly ITWJAppDbContext _context;

        public UpdateBannerCommandValidator(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            Validations();
        }

        private void Validations()
        {
        }
    }
}