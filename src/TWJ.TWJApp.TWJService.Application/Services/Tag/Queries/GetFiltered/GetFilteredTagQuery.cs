using MediatR;
using TWJ.TWJApp.TWJService.Application.Dto.Commands.Base;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;
using TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Queries.GetFiltered;

namespace TWJ.TWJApp.TWJService.Application.Services.Tag.Queries.GetFiltered
{
    public class GetFilteredTagQuery : FilterRequest, IRequest<FilterResponse<GetFilteredTagModel>>
    {
    }
}
