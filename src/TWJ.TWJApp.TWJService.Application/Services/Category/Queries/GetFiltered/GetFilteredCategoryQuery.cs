using MediatR;
using TWJ.TWJApp.TWJService.Application.Dto.Commands.Base;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Category.Queries.GetFiltered
{
    public class GetFilteredCategoryQuery : FilterRequest, IRequest<FilterResponse<GetFilteredCategoryModel>>
    {
    }
}