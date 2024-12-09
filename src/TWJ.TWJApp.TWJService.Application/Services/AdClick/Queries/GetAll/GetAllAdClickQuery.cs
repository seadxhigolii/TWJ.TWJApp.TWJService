using MediatR;
using System.Collections.Generic;

namespace TWJ.TWJApp.TWJService.Application.Services.AdClick.Queries.GetAll
{
    public class GetAllAdClickQuery : IRequest<IList<GetAllAdClickModel>>
    {
    }
}
