using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace TWJ.TWJApp.TWJService.Application.Services.FeaturedAdClick.Commands.Add
{
    public class AddFeaturedAdClickCommandHandler : IRequestHandler<AddFeaturedAdClickCommand, Unit>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public AddFeaturedAdClickCommandHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(AddFeaturedAdClickCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.UserSessionId = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                await _context.FeaturedAdClicks.AddAsync(request.AddFeaturedAdClick(), cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }
    }
}
