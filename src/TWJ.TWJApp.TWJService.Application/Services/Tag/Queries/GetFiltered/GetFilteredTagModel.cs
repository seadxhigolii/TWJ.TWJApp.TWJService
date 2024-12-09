using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.Tag.Queries.GetFiltered
{
    public class GetFilteredTagModel : IProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PostCount { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.Tag, GetFilteredTagModel>(
                (src, options) =>
                {
                    return new GetFilteredTagModel
                    {
                        Id = src.Id,
                        Name = src.Name,
                        PostCount = src.PostCount
                    };
                });
        }
    }
}
