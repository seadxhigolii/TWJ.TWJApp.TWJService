using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.News.Queries.GetAll
{
    public class GetAllNewsModel : IProfile
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<Domain.Entities.News, GetAllNewsModel>(
                (src, options) =>
                {
                    return new GetAllNewsModel
                    {
                        Title = src.Title,
                        Description = src.Description
                    };
                });
        }
    }
}