using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Queries.GetById
{
    public class GetNewsLetterSubscriberByIdQuery : IRequest<GetNewsLetterSubscriberByIdModel>
    {
        public Guid Id { get; set; }
    }
}