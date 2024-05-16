using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.BlogPostCategory.Queries.GetAll
{
    public class GetAllBlogPostCategoryModel : IProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<Domain.Entities.BlogPostCategory, GetAllBlogPostCategoryModel>(
                (src, options) =>
                {
                    return new GetAllBlogPostCategoryModel
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