using WebScraper.Entities;

namespace WebScraper.Interfaces
{
    public interface IScienceDailyScrapperService
    {
        Task<IList<NewsDataItem>> ScrapeDataAsync(string url);
    }
}
