
using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetFiltered
{
    public class GetFilteredBlogPostModel : IProfile
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductCategoryId { get; set; }
        public Guid BlogPostCategoryId { get; set; }
        public string Tags { get; set; }
        public string Image { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int NumberOfComments { get; set; }
        public Guid? ProductID { get; set; }
        public string AuthorName { get; set; }
        public string AuthorImage { get; set; }
        public string URL { get; set; }
        public DateTime CreatedAt { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.BlogPost, GetFilteredBlogPostModel>(
                (src, options) =>
                {
                    return new GetFilteredBlogPostModel
                    {
                        Id = src.Id,
                        Title = src.Title,
                        UserId = src.UserId,
                        ProductCategoryId = src.ProductCategoryId,
                        Tags = src.Tags,
                        Image = src.Image,
                        Views = src.Views,
                        Likes = src.Likes,
                        Dislikes = src.Dislikes,
                        NumberOfComments = src.NumberOfComments,
                        ProductID = src.ProductID,
                        URL = src.URL,
                        CreatedAt = src.CreatedAt
                    };
                });
        }
    }
}