﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using WebScraper.Entities;
using WebScraper.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.MedicalXpress
{
    public class MedicalXpressService : IMedicalXpressService
    {
        private readonly WebScraper.Interfaces.IMedicalXpressScrapperService _medicalXpressScrapperService;
        public MedicalXpressService(WebScraper.Interfaces.IMedicalXpressScrapperService medicalXpressScrapperService) 
        { 
            _medicalXpressScrapperService = medicalXpressScrapperService;
        }
        public async Task<IList<NewsDataItem>> PerformWebScrapAsync(string url)
        {
            var newsList = await _medicalXpressScrapperService.ScrapeDataAsync(url);
            return newsList;
        }
    }
}
