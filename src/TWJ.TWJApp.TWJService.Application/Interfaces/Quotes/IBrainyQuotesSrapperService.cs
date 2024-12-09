using System.Collections.Generic;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Application.Interfaces.Quotes
{
    public interface IBrainyQuotesSrapperService
    {
        Task<IList<QuotesDataItem>> ScrapeDataAsync(string url);
    }
}
