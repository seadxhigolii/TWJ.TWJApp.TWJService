using MediatR;
using System.Collections.Generic;

namespace TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Queries.GetAll
{
    public class GetAllNewsLetterSubscriberQuery : IRequest<IList<GetAllNewsLetterSubscriberModel>>
    {
    }
}