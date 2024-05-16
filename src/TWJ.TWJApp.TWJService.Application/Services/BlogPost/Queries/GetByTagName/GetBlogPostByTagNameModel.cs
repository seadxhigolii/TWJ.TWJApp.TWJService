using MapperSegregator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetByTagName
{
    public class GetBlogPostByTagNameModel : IProfile
    {

        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid UserId { get; set; }
        public string Image { get; set; }
        public string URL { get; set; }
        public DateTime CreatedAt { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.BlogPost, GetBlogPostByTagNameModel>(
                (src, options) =>
                {
                    return new GetBlogPostByTagNameModel
                    {
                        Id = src.Id,
                        Title = src.Title,
                        URL = src.URL,
                        UserId = src.UserId,
                        Image = src.Image,
                        CreatedAt = src.CreatedAt
                    };
                });
        }
    }
}
