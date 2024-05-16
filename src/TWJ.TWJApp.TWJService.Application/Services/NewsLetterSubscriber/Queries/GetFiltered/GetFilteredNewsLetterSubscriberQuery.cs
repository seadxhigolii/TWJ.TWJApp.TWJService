using MediatR;
using TWJ.TWJApp.TWJService.Application.Dto.Commands.Base;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.NewsLetterSubscriber.Queries.GetFiltered
{
    public class GetFilteredNewsLetterSubscriberQuery : FilterRequest, IRequest<FilterResponse<GetFilteredNewsLetterSubscriberModel>>
    {
    }
}