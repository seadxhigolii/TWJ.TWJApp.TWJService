using System.Collections.Generic;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using WebScraper.Entities;

namespace TWJ.TWJApp.TWJService.Application.Services.MedicalNewsToday
{
    public class MedicalNewsTodayScrapperService : IMedicalNewsTodayScrapperService
    {
        private readonly WebScraper.Interfaces.IMedicalNewsTodayScrapperService _medicalNewsTodayScrapperService;
        public MedicalNewsTodayScrapperService(WebScraper.Interfaces.IMedicalNewsTodayScrapperService medicalNewsTodayScrapperService)
        {
            _medicalNewsTodayScrapperService = medicalNewsTodayScrapperService;
        }
        public async Task<IList<NewsDataItem>> PerformWebScrapAsync(string url)
        {
            var newsList = await _medicalNewsTodayScrapperService.ScrapeDataAsync(url);
            return newsList;
        }
    }
}
