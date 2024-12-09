using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Models;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.Preplexity
{
    public class PreplexityService : IPreplexityService
    {
        private readonly HttpClient _httpClient;
        private string apiKey;
        private string baseUrl;
        private string _model;

        public PreplexityService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            apiKey = configuration["Preplexity:Key"];
            baseUrl = configuration["Preplexity:BaseURL"];
            _model = configuration["Preplexity:Model"];
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<string[]> GenerateTitleAsync(string topic)
        {
            var payload = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "user", content = $"{topic}" }
                },
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{baseUrl}/chat/completions", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<PreplexityResponse>(jsonResponse);

                    if (responseObject != null && responseObject.Choices != null && responseObject.Choices.Count > 0)
                    {
                        var choiceContent = responseObject.Choices[0].Message.Content;

                        choiceContent = choiceContent.Replace("\n", string.Empty);

                        var titles = new List<string>();
                        var matches = Regex.Matches(choiceContent, "\"([^\"]*)\"");

                        foreach (Match match in matches)
                        {
                            titles.Add(match.Groups[1].Value);
                        }

                        return titles.ToArray();
                    }
                }

                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(errorContent);
                }
            }
            catch (HttpRequestException e) 
            {
                Debug.WriteLine($"HTTP Request Error: {e.Message}");
                if (e.InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception: {e.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
            return new string[0];
        }
        public async Task<string> GenerateContentAsync(string topic)
        {
            var payload = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "user", content = $"{topic}" }
                },
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{baseUrl}/chat/completions", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<PreplexityResponse>(jsonResponse);

                    if (responseObject != null && responseObject.Choices != null && responseObject.Choices.Count > 0)
                    {
                        var choiceContent = responseObject.Choices[0].Message.Content;


                        return choiceContent;
                    }

                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(errorContent);
                }
            }
            catch (HttpRequestException e) 
            {
                throw e;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }

    }
}
