using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Domain.Entities.OpenAi
{
    public class ChatCompletionResponse
    {
        public string Id { get; set; } 
        public string Object { get; set; }
        public long Created { get; set; } 
        public string Model { get; set; }
        public List<ChatCompletionChoice> Choices { get; set; }
    }

    public class ChatCompletionChoice
    {
        public int Index { get; set; }
        public ChatMessage Message { get; set; }
    }


}
