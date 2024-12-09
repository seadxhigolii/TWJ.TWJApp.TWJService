
using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetAll
{
    public class GetAllBlogPostModel : IProfile
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

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.BlogPost, GetAllBlogPostModel>(
                (src, options) =>
                {
                    return new GetAllBlogPostModel
                    {
                        Id = src.Id,
                        Title = src.Title,
                        UserId = src.UserId,
                        ProductCategoryId = src.ProductCategoryId,
                        CreatedAt = src.CreatedAt,
                        Image = src.Image,
                        Views = src.Views,
                        URL = src.URL
                    };
                });
        }
    }
}