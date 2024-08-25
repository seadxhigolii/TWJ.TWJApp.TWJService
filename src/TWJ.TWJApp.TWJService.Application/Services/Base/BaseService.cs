using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.Base
{
    public class BaseService
    {
        protected readonly IConfiguration _configuration;
        protected readonly IGlobalHelperService _globalHelperService;
        protected readonly IOpenAiService _openAiService;
        protected readonly string _environment;
        protected string className;
        public BaseService(IConfiguration configuration, IGlobalHelperService globalHelperService, IOpenAiService openAiService)
        {
            _configuration = configuration;
            _globalHelperService = globalHelperService;
            _openAiService = openAiService;
            _environment = _configuration["Environment"];
        }

        protected string GetApiDirectory()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string apiDirectory = "";

            if (_environment == "Development")
            {
                apiDirectory = Path.Combine(Directory.GetParent(currentDirectory).Parent.Parent.Parent.FullName);
            }
            else
            {
                apiDirectory = Path.Combine(currentDirectory);
            }

            return apiDirectory;
        }

        protected async Task<string> GetAIResponse(string prompt)
        {
            var result = await _openAiService.GenerateSectionAsync(prompt);
            return result;
        }
    }
}
