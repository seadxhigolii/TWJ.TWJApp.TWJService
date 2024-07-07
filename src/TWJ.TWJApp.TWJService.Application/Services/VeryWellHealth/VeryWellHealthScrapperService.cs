using System.Collections.Generic;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using WebScraper.Entities;

namespace TWJ.TWJApp.TWJService.Application.Services.VeryWellHealth
{
    public class VeryWellHealthScrapperService : IVeryWellHealthScrapperService
    {
        private readonly WebScraper.Interfaces.IVeryWellHealthScrapperService _veryWellHealthScrapperService;
        public VeryWellHealthScrapperService(WebScraper.Interfaces.IVeryWellHealthScrapperService veryWellHealthScrapperService)
        {
            _veryWellHealthScrapperService = veryWellHealthScrapperService;
        }
        public async Task<IList<NewsDataItem>> PerformWebScrapAsync(string url)
        {
            var newsList = await _veryWellHealthScrapperService.ScrapeDataAsync(url);
            return newsList;
        }
    }
}
