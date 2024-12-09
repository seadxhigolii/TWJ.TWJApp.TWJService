using MapperSegregator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetByUrl;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetRelated
{
    public class GetRelatedBlogPostModel : IProfile
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid ProductCategoryId { get; set; }
        public Guid BlogPostCategoryId { get; set; }
        public string Image { get; set; }
        public int Views { get; set; }
        public string Content { get; set; }
        public string Tags { get; set; }
        public string AuthorName { get; set; }
        public string AuthorImage { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.BlogPost, GetRelatedBlogPostModel>(
                (src, options) =>
                {
                    return new GetRelatedBlogPostModel
                    {
                        Id = src.Id,
                        Title = src.Title,
                        UserId = src.UserId,
                        ProductCategoryId = src.ProductCategoryId,
                        CreatedAt = src.CreatedAt,
                        Image = src.Image,
                        Views = src.Views,
                        URL = src.URL,
                        Content = src.Content,
                        Tags = src.Tags,
                    };
                });
        }
    }
}
