using MapperSegregator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetTopTags
{
    public class GetTopTagsBlogPostModel
    {
        public string TagName { get; set; }
        public Guid TagID { get; set; }
        public IList<BlogPostDto> BlogPosts { get; set; }
    }

    public class BlogPostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string ImageURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public string URL { get; set; }
    }

    public class CombinedBlogPostModel
    {
        public IList<BlogPostDto> LatestPosts { get; set; }
        public IList<GetTopTagsBlogPostModel> TopTagsWithPosts { get; set; }
    }
}
