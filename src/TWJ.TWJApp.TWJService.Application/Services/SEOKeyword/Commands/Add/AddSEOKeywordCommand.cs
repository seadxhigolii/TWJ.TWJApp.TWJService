using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Commands.Add
{
    public class AddSEOKeywordCommand : IRequest<Unit>
    {
        public string Keyword { get; set; }
        public int SearchVolume { get; set; }
        public int CompetitionLevel { get; set; }
        public Guid CategoryId { get; set; }

        public TWJ.TWJApp.TWJService.Domain.Entities.SEOKeyword AddSEOKeyword()
        {
            return new TWJ.TWJApp.TWJService.Domain.Entities.SEOKeyword
            {
                Keyword = Keyword,
                SearchVolume = SearchVolume,
                CompetitionLevel = CompetitionLevel,
                CategoryId = CategoryId
            };
        }
    }
}