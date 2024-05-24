using WebScraper.Entities;

namespace WebScraper.Interfaces
{
    public interface IVeryWellHealthScrapperService
    {
        Task<IList<NewsDataItem>> ScrapeDataAsync(string url);
    }
}
