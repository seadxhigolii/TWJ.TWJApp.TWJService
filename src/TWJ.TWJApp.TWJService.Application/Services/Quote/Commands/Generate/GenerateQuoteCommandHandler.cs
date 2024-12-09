using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces.Quotes;

namespace TWJ.TWJApp.TWJService.Application.Services.Quote.Commands.Generate
{
    public class GenerateQuoteCommandHandler : IRequestHandler<GenerateQuoteCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IGlobalHelperService _globalHelper;
        private readonly IPreplexityService _preplexityService;
        private readonly IBrainyQuotesSrapperService _brainyQuotesSrapperService;
        private readonly string currentClassName = "";
        string healthBrainyQuotesURL;
        string motivationalBrainyQuotesURL;
        string peaceBrainyQuotesURL;

        public GenerateQuoteCommandHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper, IPreplexityService preplexityService, IConfiguration configuration, IMedicalXpressService medicalXpressSerivce, IVeryWellHealthScrapperService veryWellHealthScrapperService, IScienceDailyScrapperService scienceDailyScrapperService, IMedicalNewsTodayScrapperService medicalNewsTodayScrapperService, IBrainyQuotesSrapperService brainyQuotesSrapperService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
            _configuration = configuration;
            _preplexityService = preplexityService;
            healthBrainyQuotesURL = configuration["Quotes:Health:BrainyQuotes"];
            motivationalBrainyQuotesURL = configuration["Quotes:Motivational:BrainyQuotes"];
            peaceBrainyQuotesURL = configuration["Quotes:Peace:BrainyQuotes"];
            _brainyQuotesSrapperService = brainyQuotesSrapperService;
        }

        public async Task<Unit> Handle(GenerateQuoteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                for (int i = 1; i <= 17; i++)
                {
                    string pageUrl = $"{peaceBrainyQuotesURL}_{i}";

                    var quoteList = await _brainyQuotesSrapperService.ScrapeDataAsync(pageUrl);

                    foreach (var quote in quoteList)
                    {
                        var isDuplicate = await _context.Quotes
                            .AsNoTracking()
                            .AnyAsync(x => x.Content.ToLower() == quote.Quote.ToLower(), cancellationToken);

                        if (isDuplicate)
                        {
                            continue;
                        }

                        var quoteRecord = new Domain.Entities.Quote
                        {
                            Content = quote.Quote,
                            Author = quote.Author,
                            Category = "Motivational"
                        };
                        await _context.Quotes.AddAsync(quoteRecord, cancellationToken);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
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
