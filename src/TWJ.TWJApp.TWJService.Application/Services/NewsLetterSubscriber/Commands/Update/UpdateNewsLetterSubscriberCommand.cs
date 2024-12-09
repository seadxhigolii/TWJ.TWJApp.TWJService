using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Commands.Update
{
    public class UpdateNewsLetterSubscriberCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public Domain.Entities.NewsLetterSubscriber Update(Domain.Entities.NewsLetterSubscriber newslettersubscriber)
        {
            newslettersubscriber.Email = Email;
            return newslettersubscriber;
        }
    }
}