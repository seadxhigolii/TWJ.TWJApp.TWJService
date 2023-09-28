using MediatR;
using System.Collections.Generic;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Queries.GetAll
{
    public class GetAllBaseQuery : IRequest<IList<GetAllBaseModel>>
    {
    }
}
