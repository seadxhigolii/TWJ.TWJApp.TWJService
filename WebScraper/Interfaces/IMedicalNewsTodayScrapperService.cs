using WebScraper.Entities;

namespace WebScraper.Interfaces
{
    public interface IMedicalNewsTodayScrapperService
    {
        Task<IList<NewsDataItem>> ScrapeDataAsync(string url);
    }
}
