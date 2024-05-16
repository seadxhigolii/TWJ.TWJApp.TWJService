using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Commands.Update
{
    public class UpdateNewsLetterSubscriberCommandHandler : IRequestHandler<UpdateNewsLetterSubscriberCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public UpdateNewsLetterSubscriberCommandHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<Unit> Handle(UpdateNewsLetterSubscriberCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.NewsLetterSubscribers.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                _context.NewsLetterSubscribers.Update(request.Update(data));

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