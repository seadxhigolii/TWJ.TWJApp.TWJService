
using MediatR;
using System.Collections.Generic;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetAll
{
    public class GetAllBlogPostQuery : IRequest<IList<GetAllBlogPostModel>>
    {
    }
}