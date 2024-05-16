using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Services.BlogPostCategory.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPostCategory.Queries.GetById
{
    public class GetBlogPostCategoryByIdModel : IProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<Domain.Entities.BlogPostCategory, GetBlogPostCategoryByIdModel>(
                (src, options) =>
                {
                    return new GetBlogPostCategoryByIdModel
                    {
                        Id = src.Id,
                        Name = src.Name,
                        Description = src.Description,
                        URL = src.URL,
                    };
                });
        }
    }
}