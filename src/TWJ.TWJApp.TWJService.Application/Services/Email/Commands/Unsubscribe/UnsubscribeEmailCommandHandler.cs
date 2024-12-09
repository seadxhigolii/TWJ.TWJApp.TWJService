using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Models;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.Email.Commands.Unsubscribe
{
    public class UnsubscribeEmailCommandHandler : IRequestHandler<UnsubscribeEmailCommand, Response<bool>>
    {
        private readonly ITWJAppDbContext _context;

        public UnsubscribeEmailCommandHandler(ITWJAppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Response<bool>> Handle(UnsubscribeEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var subscriber = await _context.NewsLetterSubscribers.FindAsync(Guid.Parse(request.SubscriberId));
                subscriber.Subscribed = false;
                _context.NewsLetterSubscribers.Update(subscriber);
                await _context.SaveChangesAsync();

                var response = new Response<bool>
                {
                    Data = true,
                    Message = "Unsubscribed.",
                    StatusCode = 200,
                    Success = true
                };
                return response;
            }
            catch (Exception ex)
            {

                var errorResponse = new Response<bool>
                {
                    Data = false,
                    Message = ex.Message,
                    StatusCode = 500,
                    Success = false
                };
                return errorResponse;
            }
        }
    }
}
