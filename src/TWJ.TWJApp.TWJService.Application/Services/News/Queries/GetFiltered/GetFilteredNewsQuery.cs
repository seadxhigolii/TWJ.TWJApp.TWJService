using MediatR;
using TWJ.TWJApp.TWJService.Application.Dto.Commands.Base;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.News.Queries.GetFiltered
{
    public class GetFilteredNewsQuery : FilterRequest, IRequest<FilterResponse<GetFilteredNewsModel>>
    {
    }
}