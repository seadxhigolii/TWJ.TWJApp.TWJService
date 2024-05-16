using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetByUrl;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetRelated
{
    public class GetRelatedBlogPostQuery : IRequest<IList<GetRelatedBlogPostModel>>
    {
        public string URL { get; set; }
    }
}
