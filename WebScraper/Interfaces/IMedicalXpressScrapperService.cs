using WebScraper.Entities;

namespace WebScraper.Interfaces
{
    public interface IMedicalXpressScrapperService
    {
        Task<IList<NewsDataItem>> ScrapeDataAsync(string url);
    }
}
