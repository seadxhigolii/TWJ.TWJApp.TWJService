using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.Tag.Queries.GetAll
{
    public class GetAllTagModel : IProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PostCount { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.Tag, GetAllTagModel>(
                (src, options) =>
                {
                    return new GetAllTagModel
                    {
                        Id = src.Id,
                        Name = src.Name,
                        PostCount = src.PostCount
                    };
                });
        }
    }
}
