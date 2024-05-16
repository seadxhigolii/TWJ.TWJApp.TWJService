using MediatR;
using TWJ.TWJApp.TWJService.Application.Dto.Models;

namespace TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Commands.Add
{
    public class AddNewsLetterSubscriberCommand : IRequest<Response<bool>>
    {
        public string Email { get; set; }

        public Domain.Entities.NewsLetterSubscriber AddNewsLetterSubscriber()
        {
            return new Domain.Entities.NewsLetterSubscriber
            {
                Email = Email,
                Subscribed = true
            };
        }
    }
}