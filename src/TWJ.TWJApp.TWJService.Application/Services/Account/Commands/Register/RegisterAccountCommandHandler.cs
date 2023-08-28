using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Exceptions;
using TWJ.TWJApp.TWJService.Domain.Entities;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace TWJ.TWJApp.TWJService.Application.Services.Account.Commands.Register
{
    public class RegisterAccountCommandHandler : IRequestHandler<RegisterAccountCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IConfiguration _configuration;

        public RegisterAccountCommandHandler(ITWJAppDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<Unit> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (await IsEmailOrUsernameTaken(request.Email, request.UserName))
                {
                    throw new BadRequestException("Email or username is already taken.");
                }

                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserName = request.UserName,
                    Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Email = request.Email,
                    EmailConfirmed = false,
                    isActive = true,
                    DateOfBirth = request.DateOfBirth,
                    City = request.City,
                    Country = request.Country
                };

                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task<bool> IsEmailOrUsernameTaken(string email, string username)
        {
            var emailExists = await _context.Users.AnyAsync(u => u.Email == email);
            var usernameExists = await _context.Users.AnyAsync(u => u.UserName == username);
            return emailExists || usernameExists;
        }
    }
}
