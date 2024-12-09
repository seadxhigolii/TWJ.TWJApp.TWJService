using MediatR;
using TWJ.TWJApp.TWJService.Application.Dto.Commands.Base;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Queries.GetFiltered
{
    public class GetFilteredBaseQuery : FilterRequest, IRequest<FilterResponse<GetFilteredBaseModel>>
    {
    }
}
