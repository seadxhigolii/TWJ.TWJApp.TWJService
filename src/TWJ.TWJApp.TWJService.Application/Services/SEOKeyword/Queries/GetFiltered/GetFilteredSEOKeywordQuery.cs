using MediatR;
using TWJ.TWJApp.TWJService.Application.Dto.Commands.Base;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Queries.GetFiltered
{
    public class GetFilteredSEOKeywordQuery : FilterRequest, IRequest<FilterResponse<GetFilteredSEOKeywordModel>>
    {
    }
}