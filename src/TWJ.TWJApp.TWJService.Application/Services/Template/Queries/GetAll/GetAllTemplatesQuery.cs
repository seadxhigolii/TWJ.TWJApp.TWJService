using MediatR;
using System.Collections.Generic;

namespace TWJ.TWJApp.TWJService.Application.Services.Template.Queries.GetAll
{
    public class GetAllTemplatesQuery : IRequest<IList<GetAllTemplatesModel>>
    {
    }
}
