using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Queries.GetFiltered
{
    public class GetFilteredSEOKeywordModel : IProfile
    {
        public Guid Id { get; set; }
        public string Keyword { get; set; }
        public int SearchVolume { get; set; }
        public int CompetitionLevel { get; set; }
        public Guid CategoryId { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.SEOKeyword, GetFilteredSEOKeywordModel>(
                (src, options) =>
                {
                    return new GetFilteredSEOKeywordModel
                    {
                        Id = src.Id,
                        Keyword = src.Keyword,
                        SearchVolume = src.SearchVolume,
                        CategoryId = src.CategoryId
                    };
                });
        }
    }
}