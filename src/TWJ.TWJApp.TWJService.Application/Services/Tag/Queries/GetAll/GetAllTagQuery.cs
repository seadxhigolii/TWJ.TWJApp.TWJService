using MediatR;
using System.Collections.Generic;

namespace TWJ.TWJApp.TWJService.Application.Services.Tag.Queries.GetAll
{
    public class GetAllTagQuery : IRequest<IList<GetAllTagModel>>
    {
    }
}
