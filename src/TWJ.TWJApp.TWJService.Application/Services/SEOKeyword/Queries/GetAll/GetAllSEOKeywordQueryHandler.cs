using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Queries.GetAll
{
    public class GetAllSEOKeywordQueryHandler : IRequestHandler<GetAllSEOKeywordQuery, IList<GetAllSEOKeywordModel>>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public GetAllSEOKeywordQueryHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<IList<GetAllSEOKeywordModel>> Handle(GetAllSEOKeywordQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _context.SEOKeywords
                                    .AsNoTracking()
                                    .OrderByDescending(x => x.CompetitionLevel)
                                    .ToListAsync(cancellationToken);

                var mappedData = data.Select(t => new GetAllSEOKeywordModel
                {
                    Id = t.Id,
                    Keyword = t.Keyword,
                    SearchVolume = t.SearchVolume,
                    CompetitionLevel = t.CompetitionLevel,
                    CategoryId= t.CategoryId
                }).ToList();
                return mappedData;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }
    }
}