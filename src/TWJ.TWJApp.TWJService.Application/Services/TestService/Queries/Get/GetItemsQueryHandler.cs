using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.TestService.Queries.Get
{
    public class GetItemsQueryHandler : IRequestHandler<GetItemsQuery, IList<GetItemsModel>>
    {
        public async Task<IList<GetItemsModel>> Handle(GetItemsQuery request, CancellationToken cancellationToken)
        {
            await Task.Delay(500, cancellationToken);

            return new List<GetItemsModel>
            {
                new GetItemsModel { Name = "Test" },
                new GetItemsModel { Name = "Test1" }
            };
        }
    }
}
