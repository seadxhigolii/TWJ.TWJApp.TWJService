using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;
using TWJ.TWJApp.TWJService.Common.Exceptions;
using TWJ.TWJApp.TWJService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Caching.Memory;


namespace TWJ.TWJApp.TWJService.Application.Services.Account.Commands.Login
{
    public class LoginAccountCommandHandler : IRequestHandler<LoginAccountCommand, LoginAccountModel>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        public LoginAccountCommandHandler(ITWJAppDbContext context, IConfiguration configuration, IMemoryCache cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _cache = cache;
        }

        public async Task<LoginAccountModel> Handle(LoginAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await GetUser(request);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (!ValidatePassword(user, request.Password))
                throw new BadRequestException(ValidatorMessages.IncorrectPassword);

            var (token, expire) = await GetAccessToken(user);

            var cacheKey = user.Id.ToString(); 
            _cache.Set(cacheKey, user, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
            });

            return new LoginAccountModel
            {
                UserId = user.Id.ToString(),
                Token = token,
                ExpireDate = expire
            };
        }


        public bool ValidatePassword(Domain.Entities.User user, string enteredPassword)
        {
            string hashedPassword = user.Password;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(enteredPassword, hashedPassword);

            return isPasswordValid;
        }
        private async Task<(string token, DateTime expire)> GetAccessToken(Domain.Entities.User user)
        {
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:Token"])), SecurityAlgorithms.HmacSha512);

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(await GetIdentityClaims(user)),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = signingCredentials
            };

            return (tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor)), tokenDescriptor.Expires.Value);
        }

        private static Task<List<Claim>> GetIdentityClaims(Domain.Entities.User user)
        {
            List<Claim> claims = new();

            claims.AddRange(new List<Claim>
            {
                new Claim(UserClaimEnum.UserId, user.Id.ToString() ?? string.Empty),
                new Claim(UserClaimEnum.UserName, user.UserName ?? string.Empty),
                new Claim(UserClaimEnum.Email, user.Email ?? string.Empty),
                new Claim(UserClaimEnum.FirstName, user.FirstName ?? string.Empty),
                new Claim(UserClaimEnum.LastName, user.LastName ?? string.Empty)
            });

            return Task.FromResult(claims);
        }

        private async Task<Domain.Entities.User> GetUser(LoginAccountCommand request)
        {
            Domain.Entities.User user;
            if (new Regex(ValidatorRegex.Email).IsMatch(request.EmailOrUsername))
                user = await _context.User.FirstOrDefaultAsync(x => x.Email.ToLower() == request.EmailOrUsername.ToLower());
            else
                user = await _context.User.FirstOrDefaultAsync(x => x.UserName.ToLower() == request.EmailOrUsername.ToLower());
            if(user == null)
                throw new ArgumentNullException(nameof(user));

            return user ?? throw new BadRequestException(ValidatorMessages.NotFound($"Username or Email {request.EmailOrUsername}"));
        }
    }
}
