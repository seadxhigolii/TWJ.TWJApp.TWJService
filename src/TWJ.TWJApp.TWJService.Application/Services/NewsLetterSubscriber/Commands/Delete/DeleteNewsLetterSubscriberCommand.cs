using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Commands.Delete
{
    public class DeleteNewsLetterSubscriberCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}