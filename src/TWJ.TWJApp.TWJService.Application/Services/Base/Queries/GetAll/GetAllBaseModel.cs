using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Queries.GetAll
{
    public class GetAllBaseModel : IProfile
    {
        public Guid Id { get; set; }
        public string Property { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<BaseModel, GetAllBaseModel>(
                (src, options) =>
                {
                    return new GetAllBaseModel
                    {
                        Id = src.Id,
                        Property = src.Property,
                    };
                });
        }
    }
}
