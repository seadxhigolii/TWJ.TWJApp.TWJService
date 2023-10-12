
using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetFiltered
{
    public class GetFilteredBlogPostModel : IProfile
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public string Tags { get; set; }
        public Byte[] Image { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int NumberOfComments { get; set; }
        public Guid? ProductID { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.BlogPost, GetFilteredBlogPostModel>(
                (src, options) =>
                {
                    return new GetFilteredBlogPostModel
                    {
                        Id = src.Id,
                        Title = src.Title,
                        Content = src.Content,
                        UserId = src.UserId,
                        CategoryId = src.CategoryId,
                        Tags = src.Tags,
                        Image = src.Image,
                        Views = src.Views,
                        Likes = src.Likes,
                        Dislikes = src.Dislikes,
                        NumberOfComments = src.NumberOfComments,
                        ProductID = src.ProductID
                    };
                });
        }
    }
}