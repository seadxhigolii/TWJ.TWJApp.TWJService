using MediatR;
using TWJ.TWJApp.TWJService.Application.Dto.Models;

namespace TWJ.TWJApp.TWJService.Application.Services.Email.Commands.Unsubscribe
{
    public class UnsubscribeEmailCommand : IRequest<Response<bool>>
    {
        public string SubscriberId { get; set; }
    }
}
