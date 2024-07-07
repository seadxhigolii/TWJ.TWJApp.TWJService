using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces.Quotes;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Application.Services.Quotes.BrainyQuotes
{
    public class BrainyQuotesScrapperService : IBrainyQuotesSrapperService
    {
        public async Task<IList<QuotesDataItem>> ScrapeDataAsync(string url)
        {
            List<QuotesDataItem> scrapedData = new List<QuotesDataItem>();
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(url);

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("div[id^='pos_']")));

                var quoteDivs = driver.FindElements(By.CssSelector("div[id^='pos_']"));

                foreach (var div in quoteDivs)
                {
                    try
                    {
                        var quoteLink = div.FindElement(By.CssSelector("a[title='view quote']"));
                        var quoteText = quoteLink.FindElement(By.CssSelector("div")).Text;

                        var authorLink = div.FindElement(By.CssSelector("a[title='view author']"));
                        var authorText = authorLink.Text;

                        scrapedData.Add(new QuotesDataItem
                        {
                            Quote = quoteText,
                            Author = authorText
                        });

                        Debug.WriteLine($"Quote: {quoteText}, Author: {authorText}");
                    }
                    catch (NoSuchElementException)
                    {
                        continue;
                    }
                }

                driver.Quit();
            }

            return scrapedData;
        }
    }
}
