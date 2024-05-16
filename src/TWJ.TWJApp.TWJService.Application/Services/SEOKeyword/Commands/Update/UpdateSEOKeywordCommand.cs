using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Commands.Update
{
    public class UpdateSEOKeywordCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Keyword { get; set; }
        public int CompetitionLevel { get; set; }
        public Guid CategoryId { get; set; }

        public TWJ.TWJApp.TWJService.Domain.Entities.SEOKeyword Update(TWJ.TWJApp.TWJService.Domain.Entities.SEOKeyword seokeyword)
        {
            seokeyword.Keyword = Keyword;
            seokeyword.CompetitionLevel = CompetitionLevel;
            seokeyword.CategoryId = CategoryId;
            seokeyword.UpdatedAt = DateTime.Now;
            return seokeyword;
        }
    }
}