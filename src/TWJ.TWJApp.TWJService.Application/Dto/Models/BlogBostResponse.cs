using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Dto.Models
{
    public class BlogPostResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string HtmlContent { get; set; }
        public Guid BlogPostCategoryId { get; set; }
        public Guid ProductId { get; set; }
        public string BackLinkKeywords { get; set; }
        public string URL { get; set; }
        public string Image { get; set; }
    }
}
