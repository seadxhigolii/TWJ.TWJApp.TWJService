using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;
using TWJ.TWJApp.TWJService.Common.Exceptions;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using TWJ.TWJApp.TWJService.Domain.Enums;
using BCrypt.Net;


namespace TWJ.TWJApp.TWJService.Application.Services.Account.Commands.Login
{
    public class LoginAccountCommandHandler : IRequestHandler<LoginAccountCommand, LoginAccountModel>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IConfiguration _configuration;
        public LoginAccountCommandHandler(ITWJAppDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<LoginAccountModel> Handle(LoginAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await GetUser(request);
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (!ValidatePassword(user, request.Password))
                throw new BadRequestException(ValidatorMessages.IncorrectPassword);

            var (token, expire) = await GetAccessToken(user);

            return new LoginAccountModel
            {
                Token = token,
                ExpireDate = expire
            };
        }

        public bool ValidatePassword(User user, string enteredPassword)
        {
            string hashedPassword = user.Password;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(enteredPassword, hashedPassword);

            return isPasswordValid;
        }
        private async Task<(string token, DateTime expire)> GetAccessToken(User user)
        {
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:Token"])), SecurityAlgorithms.HmacSha512);

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(await GetIdentityClaims(user)),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = signingCredentials
            };

            return (tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor)), tokenDescriptor.Expires.Value);
        }

        private static Task<List<Claim>> GetIdentityClaims(User user)
        {
            List<Claim> claims = new();

            claims.AddRange(new List<Claim>
            {
                new Claim(UserClaimEnum.UserId, user.Id.ToString() ?? string.Empty),
                new Claim(UserClaimEnum.UserName, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(UserClaimEnum.Email, user.Email ?? string.Empty),
                new Claim(UserClaimEnum.FirstName, user.FirstName ?? string.Empty),
                new Claim(UserClaimEnum.LastName, user.LastName ?? string.Empty)
            });

            return Task.FromResult(claims);
        }

        private async Task<User> GetUser(LoginAccountCommand request)
        {
            User user;
            if (new Regex(ValidatorRegex.Email).IsMatch(request.EmailOrUsername))
                user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == request.EmailOrUsername.ToLower());
            else
                user = await _context.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == request.EmailOrUsername.ToLower());
            if(user == null)
                throw new ArgumentNullException(nameof(user));

            return user ?? throw new BadRequestException(ValidatorMessages.NotFound($"Username or Email {request.EmailOrUsername}"));
        }
    }
}
