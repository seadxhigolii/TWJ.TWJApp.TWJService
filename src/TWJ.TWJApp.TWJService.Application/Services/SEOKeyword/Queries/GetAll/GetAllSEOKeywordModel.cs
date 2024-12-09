using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Queries.GetAll
{
    public class GetAllSEOKeywordModel : IProfile
    {
        public Guid Id { get; set; }
        public string Keyword { get; set; }
        public int SearchVolume { get; set; }
        public int CompetitionLevel { get; set; }
        public Guid CategoryId { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.SEOKeyword, GetAllSEOKeywordModel>(
                (src, options) =>
                {
                    return new GetAllSEOKeywordModel
                    {
                        Id = src.Id,
                        Keyword = src.Keyword,
                        SearchVolume = src.SearchVolume,
                        CategoryId =  src.CategoryId
                    };
                });
        }
    }
}