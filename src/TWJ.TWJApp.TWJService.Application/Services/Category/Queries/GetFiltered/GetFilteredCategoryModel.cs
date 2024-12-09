using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.Category.Queries.GetFiltered
{
    public class GetFilteredCategoryModel : IProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.Category, GetFilteredCategoryModel>(
                (src, options) =>
                {
                    return new GetFilteredCategoryModel
                    {
                        Id = src.Id,
                        Name = src.Name,
                        Description = src.Description
                    };
                });
        }
    }
}