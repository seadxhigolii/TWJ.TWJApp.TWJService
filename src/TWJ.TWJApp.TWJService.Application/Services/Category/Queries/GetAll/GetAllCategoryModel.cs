using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.Category.Queries.GetAll
{
    public class GetAllCategoryModel : IProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.Category, GetAllCategoryModel>(
                (src, options) =>
                {
                    return new GetAllCategoryModel
                    {
                        Id = src.Id,
                        Name = src.Name,
                        Description = src.Description
                    };
                });
        }
    }
}