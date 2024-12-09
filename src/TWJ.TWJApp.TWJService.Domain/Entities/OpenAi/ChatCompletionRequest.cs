using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Domain.Entities.OpenAi
{
    public class ChatCompletionRequest
    {
        public string Model { get; set; }
        public List<ChatMessage> Messages { get; set; }
        public double Temperature { get; set; } 
        //public int MaxTokens { get; set; } 

        public ChatCompletionRequest()
        {
            Messages = new List<ChatMessage>();
        }
    }

}
