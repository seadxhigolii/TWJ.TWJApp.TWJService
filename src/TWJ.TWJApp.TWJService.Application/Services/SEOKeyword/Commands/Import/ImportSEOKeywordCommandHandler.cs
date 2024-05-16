using MediatR;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.Product.Commands.Import;
using CsvHelper;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TWJ.TWJApp.TWJService.Application.Services.SEOKeyword.Commands.Import
{
    public class ImportSEOKeywordCommandHandler : IRequestHandler<ImportSEOKeywordCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = "";

        public ImportSEOKeywordCommandHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }

        public async Task<Unit> Handle(ImportSEOKeywordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using (var reader = new StreamReader(request.FilePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        var keyword = csv.GetField<string>("Keyword");

                        var existingKeyword = await _context.SEOKeywords
                            .FirstOrDefaultAsync(k => k.Keyword == keyword, cancellationToken);

                        if (existingKeyword == null)
                        {
                            var searchVolume = csv.GetField<int>("SearchVolume");
                            var competitionLevel = csv.GetField<int>("CompetitionLevel");
                            var type = csv.GetField<int>("Type");

                            var newKeyword = new Domain.Entities.SEOKeyword
                            {
                                Id = Guid.NewGuid(),
                                Keyword = keyword,
                                SearchVolume = searchVolume,
                                CompetitionLevel = competitionLevel,
                                Type = (Domain.Entities.KeywordType)type,
                                ClickThroughRate = 0,
                                CategoryId = Guid.Parse("66a10c17-79de-44f5-b680-6e2e78079f1e")
                            };

                            await _context.SEOKeywords.AddAsync(newKeyword, cancellationToken);
                        }
                    }

                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Unit.Value;
            }
            catch (Exception ex)
            {
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception("An error occurred while processing the request.", ex);
            }
        }

    }
}
