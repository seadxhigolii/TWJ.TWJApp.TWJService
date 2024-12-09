using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraper.Entities;

namespace TWJ.TWJApp.TWJService.Application.Interfaces
{
    public interface IMedicalXpressService
    {
        Task<IList<NewsDataItem>> PerformWebScrapAsync(string url);
    }
}
