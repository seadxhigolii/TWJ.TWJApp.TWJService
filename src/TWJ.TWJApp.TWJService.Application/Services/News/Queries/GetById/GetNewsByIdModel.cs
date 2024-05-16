using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Services.News.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Application.Services.News.Queries.GetById
{
    public class GetNewsByIdModel : IProfile
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<Domain.Entities.News, GetNewsByIdModel>(
                (src, options) =>
                {
                    return new GetNewsByIdModel
                    {
                        Id = src.Id,
                        Title = src.Title,
                        Description = src.Description,
                    };
                });
        }
    }
}