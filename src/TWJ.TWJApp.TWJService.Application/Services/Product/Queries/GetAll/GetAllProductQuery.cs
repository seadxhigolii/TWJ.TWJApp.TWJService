using MediatR;
using System.Collections.Generic;

namespace TWJ.TWJApp.TWJService.Application.Services.Product.Queries.GetAll
{
    public class GetAllProductQuery : IRequest<IList<GetAllProductModel>>
    {
    }
}