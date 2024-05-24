using System.Collections.Generic;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using WebScraper.Entities;

namespace TWJ.TWJApp.TWJService.Application.Services.ScienceDaily
{
    public class ScienceDailyScrapperService : IScienceDailyScrapperService
    {
        private readonly WebScraper.Interfaces.IScienceDailyScrapperService _scienceDailyScrapperService;
        public ScienceDailyScrapperService(WebScraper.Interfaces.IScienceDailyScrapperService scienceDailyScrapperService)
        {
            _scienceDailyScrapperService = scienceDailyScrapperService;
        }
        public async Task<IList<NewsDataItem>> PerformWebScrapAsync(string url)
        {
            var newsList = await _scienceDailyScrapperService.ScrapeDataAsync(url);
            return newsList;
        }
    }
}
