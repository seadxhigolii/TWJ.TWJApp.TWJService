using MediatR;
using System.Collections.Generic;

namespace TWJ.TWJApp.TWJService.Application.Services.News.Queries.GetAll
{
    public class GetAllNewsQuery : IRequest<IList<GetAllNewsModel>>
    {
    }
}