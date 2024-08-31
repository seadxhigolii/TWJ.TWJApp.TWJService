using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraper.Entities;
using WebScraper.Interfaces;

namespace WebScraper.Services
{
    public class VeryWellHealthScrapperService : IVeryWellHealthScrapperService
    {
        public async Task<IList<NewsDataItem>> ScrapeDataAsync(string url)
        {
            // Set the environment variable for Selenium Manager cache directory
            Environment.SetEnvironmentVariable("SELENIUM_MANAGER_CACHE", "/var/selenium");

            List<NewsDataItem> scrapedData = new List<NewsDataItem>();
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(url);

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("span.card__title-text")));

                var newsLinks = driver.FindElements(By.CssSelector("span.card__title-text"));

                foreach (var link in newsLinks)
                {
                    var newsItem = new NewsDataItem
                    {
                        Title = link.Text
                    };
                    scrapedData.Add(newsItem);
                }

                driver.Quit();
            }

            return scrapedData;
        }

    }
}
