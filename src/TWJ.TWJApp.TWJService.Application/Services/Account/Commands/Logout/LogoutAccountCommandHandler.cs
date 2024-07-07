using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.Account.Commands.Logout
{
    public class LogoutAccountCommandHandler : IRequestHandler<LogoutAccountCommand, bool>
    {
        private readonly IMemoryCache _cache;
        public LogoutAccountCommandHandler(IMemoryCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<bool> Handle(LogoutAccountCommand request, CancellationToken cancellationToken)
        {
            _cache.Remove(request.UserId);
            return true;
        }
    }
}
