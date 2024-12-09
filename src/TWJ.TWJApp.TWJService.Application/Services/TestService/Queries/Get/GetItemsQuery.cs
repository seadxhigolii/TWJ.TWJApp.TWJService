using MediatR;
using System.Collections.Generic;

namespace TWJ.TWJApp.TWJService.Application.Services.TestService.Queries.Get
{
    public class GetItemsQuery : IRequest<IList<GetItemsModel>>
    {
    }
}
