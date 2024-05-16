using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.News.Queries.GetFiltered
{
    public class GetFilteredNewsModel : IProfile
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<Domain.Entities.News, GetFilteredNewsModel>(
                (src, options) =>
                {
                    return new GetFilteredNewsModel
                    {
                        Id = src.Id,
                        Title = src.Title,
                        Description = src.Description,
                    };
                });
        }
    }
}