using MediatR;
using TWJ.TWJApp.TWJService.Application.Dto.Commands.Base;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Banner.Queries.GetFiltered
{
    public class GetFilteredBannerQuery : FilterRequest, IRequest<FilterResponse<GetFilteredBannerModel>>
    {
    }
}