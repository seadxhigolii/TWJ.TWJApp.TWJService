using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Dto.Models;

namespace TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Commands.Add
{
    public class AddNewsLetterSubscriberCommandHandler : IRequestHandler<AddNewsLetterSubscriberCommand, Response<bool>>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public AddNewsLetterSubscriberCommandHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<Response<bool>> Handle(AddNewsLetterSubscriberCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _context.NewsLetterSubscribers.AddAsync(request.AddNewsLetterSubscriber(), cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                var response = new Response<bool>
                {
                    Data = true,
                    Message = string.Empty,
                    StatusCode = 200,
                    Success = true
                };
                return response;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }
    }
}