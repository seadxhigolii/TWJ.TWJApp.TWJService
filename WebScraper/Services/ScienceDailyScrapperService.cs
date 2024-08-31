﻿using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using WebScraper.Entities;
using WebScraper.Interfaces;

namespace WebScraper.Services
{
    public class ScienceDailyScrapperService : IScienceDailyScrapperService
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
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("summaries")));

                var newsLinks = driver.FindElements(By.CssSelector("#summaries div.latest-head a"));

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
