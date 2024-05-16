using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetByUrl
{
    public class GetBlogPostByUrlQuery : IRequest<GetBlogPostByUrlModel>
    {
        public string URL { get; set; }
    }
}
