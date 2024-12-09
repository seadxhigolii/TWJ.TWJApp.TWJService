using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Services.Category.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Application.Services.Category.Queries.GetById
{
    public class GetCategoryByIdModel : IProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description{ get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.Category, GetCategoryByIdModel>(
                (src, options) =>
                {
                    return new GetCategoryByIdModel
                    {
                        Id = src.Id,
                        Name = src.Name,
                        Description = src.Description
                    };
                });
        }
    }
}