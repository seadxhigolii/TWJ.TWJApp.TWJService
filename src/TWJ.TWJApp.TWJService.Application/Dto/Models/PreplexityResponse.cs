using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Dto.Models
{
    public class PreplexityResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("model")]
        public string Model { get; set; }
        [JsonPropertyName("created")]
        public long Created { get; set; }
        [JsonPropertyName("usage")]
        public Usage Usage { get; set; }
        [JsonPropertyName("object")]
        public string Object { get; set; }
        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }
    }

    public class Usage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }
        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }
        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }
        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }
        [JsonPropertyName("message")]
        public Message Message { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

}
