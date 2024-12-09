using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Queries.GetById
{
    public class GetSEOKeywordByIdModel : IProfile
    {
        public Guid Id { get; set; }
        public string Keyword { get; set; }
        public int SearchVolume { get; set; }
        public int CompetitionLevel { get; set; }
        public Guid CategoryId { get; set; }

        public async Task MapData(IProfileMapper profileMapper)
        {
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.SEOKeyword, GetSEOKeywordByIdModel>(
                (src, options) =>
                {
                    return new GetSEOKeywordByIdModel
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