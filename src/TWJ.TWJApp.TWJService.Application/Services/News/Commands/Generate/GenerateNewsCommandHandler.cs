using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.News.Commands.Add;

namespace TWJ.TWJApp.TWJService.Application.Services.News.Commands.Generate
{
    public class GenerateNewsCommandHandler : IRequestHandler<GenerateNewsCommand, Unit>
    {
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly IPreplexityService _preplexityService;
        private readonly IMedicalXpressService _medicalXpressSerivce;
        private readonly IMedicalNewsTodayScrapperService _medicalNewsTodayScrapperService;
        private readonly IVeryWellHealthScrapperService _veryWellHealthScrapperService;
        private readonly IScienceDailyScrapperService _scienceDailyScrapperService;
        private readonly string currentClassName = "";
        string medicalXpressURL;
        string veryWellHealthURL;
        string scienceDailyURL;
        string medicalNewsTodayURL;

        public GenerateNewsCommandHandler(ITWJAppDbContext context, IGlobalHelperService globalHelper, IPreplexityService preplexityService, IConfiguration configuration, IMedicalXpressService medicalXpressSerivce, IVeryWellHealthScrapperService veryWellHealthScrapperService, IScienceDailyScrapperService scienceDailyScrapperService, IMedicalNewsTodayScrapperService medicalNewsTodayScrapperService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
            _preplexityService = preplexityService;
            medicalNewsTodayURL = configuration["News:Health:MedicalNewsToday"];
            medicalXpressURL = configuration["News:Health:MedicalXpress"];
            veryWellHealthURL = configuration["News:Health:VeryWellHealth"];
            scienceDailyURL = configuration["News:Health:ScienceDaily"];
            _medicalNewsTodayScrapperService = medicalNewsTodayScrapperService;
            _medicalXpressSerivce = medicalXpressSerivce;
            _veryWellHealthScrapperService = veryWellHealthScrapperService;
            _scienceDailyScrapperService = scienceDailyScrapperService;
        }

        public async Task<Unit> Handle(GenerateNewsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //var newsTitleList = await _preplexityService.GenerateTitleAsync($"Give me 10 news titles (give titles only) from this URL \"{medicalXpressURL}\".");
                //var newsList = await _medicalXpressSerivce.PerformWebScrapAsync(medicalXpressURL);
                //var newsList = await _veryWellHealthScrapperService.PerformWebScrapAsync(veryWellHealthURL);
                //var newsList = await _scienceDailyScrapperService.PerformWebScrapAsync(scienceDailyURL);
                var newsList = await _medicalNewsTodayScrapperService.PerformWebScrapAsync(medicalNewsTodayURL);

                foreach (var newsTitle in newsList)
                {
                    var isDuplicate = await _context.News
                        .AsNoTracking()
                        .AnyAsync(x => x.Title.ToLower() == newsTitle.Title.ToLower(), cancellationToken);

                    if (isDuplicate)
                    {
                        continue;
                    }

                    var newsDescription = await _preplexityService.GenerateContentAsync($"Information only: '{newsTitle.Title}'. " +
                        $"No introductions, no 'sure' in the beginning, no greetings, no reiterations of the title, no use of words " +
                        $"like 'Here are some...' or 'Here is...' or something like this in the beginning. Just the facts.");

                    var newsRecord = new Domain.Entities.News
                    {
                        Title = newsTitle.Title,
                        Description = newsDescription
                    };
                    await _context.News.AddAsync(newsRecord, cancellationToken);
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
