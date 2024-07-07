using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.FeaturedAdClick.Commands.Add;

namespace TWJ.TWJApp.TWJService.Application.Services.AdClick.Commands.Add
{
    public class AddAdClickCommandHandler : IRequestHandler<AddAdClickCommand, Unit>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public AddAdClickCommandHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(AddAdClickCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.UserSessionId = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                await _context.AdClicks.AddAsync(request.AddAdClick(), cancellationToken);
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
