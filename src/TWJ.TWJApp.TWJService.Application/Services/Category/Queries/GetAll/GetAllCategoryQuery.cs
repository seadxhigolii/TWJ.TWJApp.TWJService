using MediatR;
using System.Collections.Generic;

namespace TWJ.TWJApp.TWJService.Application.Services.Category.Queries.GetAll
{
    public class GetAllCategoryQuery : IRequest<IList<GetAllCategoryModel>>
    {
    }
}