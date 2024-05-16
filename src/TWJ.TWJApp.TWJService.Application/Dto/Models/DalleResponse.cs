using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Dto.Models
{
    public class DalleResponse
    {
        [JsonPropertyName("created")]
        public long Created { get; set; }
        [JsonPropertyName("data")]
        public Data[] Data { get; set; }
    }

    public class Data
    {
        [JsonPropertyName("revised_prompt")]
        public string RevisedPrompt { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
