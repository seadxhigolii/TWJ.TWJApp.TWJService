using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebScraper.Entities;
using WebScraper.Interfaces;

namespace WebScraper.Services
{
    public class MedicalXpressScrapperService : IMedicalXpressScrapperService
    {
        public async Task<IList<NewsDataItem>> ScrapeDataAsync(string url)
        {
            List<NewsDataItem> scrapedData = new List<NewsDataItem>();
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(url);

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("h2.mb-2 a.news-link")));

                var newsLinks = driver.FindElements(By.CssSelector("h2.mb-2 a.news-link"));

                foreach (var link in newsLinks)
                {
                    var newsItem = new NewsDataItem
                    {
                        Title = link.Text, 
                        URL = link.GetAttribute("href")
                    };
                    scrapedData.Add(newsItem);
                }

                driver.Quit();
            }

            return scrapedData;
        }
    }
}
