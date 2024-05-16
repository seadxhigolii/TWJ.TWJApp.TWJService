using MediatR;
using TWJ.TWJApp.TWJService.Application.Dto.Commands.Base;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Product.Queries.GetFiltered
{
    public class GetFilteredProductQuery : FilterRequest, IRequest<FilterResponse<GetFilteredProductModel>>
    {
    }
}