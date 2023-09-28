using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Queries.GetFiltered
{
    public class GetFilteredBaseModel : IProfile
    {
        public Guid Id { get; set; }
        public string Property { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<BaseModel, GetFilteredBaseModel>(
                (src, options) =>
                {
                    return new GetFilteredBaseModel
                    {
                        Id = src.Id,
                        Property = src.Property,
                    };
                });
        }
    }
}
