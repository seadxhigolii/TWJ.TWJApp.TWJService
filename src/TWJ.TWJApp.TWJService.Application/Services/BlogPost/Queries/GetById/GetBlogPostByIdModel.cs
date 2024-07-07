using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetById;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPost.Queries.GetById
{
    public class GetBlogPostByIdModel : IProfile
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.BlogPost, GetBlogPostByIdModel>(
                (src, options) =>
                {
                    return new GetBlogPostByIdModel
                    {
                        Id = src.Id,
                        Title = src.Title,
                        Content = src.Content
                    };
                });
        }
    }
}