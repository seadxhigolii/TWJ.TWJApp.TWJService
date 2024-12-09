using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPostCategory.Queries.GetFiltered
{
    public class GetFilteredBlogPostCategoryModel : IProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<Domain.Entities.BlogPostCategory, GetFilteredBlogPostCategoryModel>(
                (src, options) =>
                {
                    return new GetFilteredBlogPostCategoryModel
                    {
                        Id = src.Id,
                        Description = src.Description,
                        URL = src.URL
                    };
                });
        }
    }
}