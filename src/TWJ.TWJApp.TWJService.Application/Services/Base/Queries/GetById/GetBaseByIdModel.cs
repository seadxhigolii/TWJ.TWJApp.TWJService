using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Services.Base.Queries.GetAll;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.Base.Queries.GetById
{
    public class GetBaseByIdModel : IProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

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
