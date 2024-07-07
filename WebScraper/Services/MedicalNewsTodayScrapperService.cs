using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using WebScraper.Entities;
using WebScraper.Interfaces;

namespace WebScraper.Services
{
    public class MedicalNewsTodayScrapperService : IMedicalNewsTodayScrapperService
    {
        public async Task<IList<NewsDataItem>> ScrapeDataAsync(string url)
        {
            List<NewsDataItem> scrapedData = new List<NewsDataItem>();
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(url);

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("a.css-1izc1yn h2")));

                var newsLinks = driver.FindElements(By.CssSelector("a.css-1izc1yn"));

                foreach (var link in newsLinks)
                {
                    var newsItem = new NewsDataItem
                    {
                        Title = link.FindElement(By.TagName("h2")).Text,
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
