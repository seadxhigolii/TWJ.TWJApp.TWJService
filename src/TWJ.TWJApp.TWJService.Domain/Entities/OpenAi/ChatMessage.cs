using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Domain.Entities.OpenAi
{
    public class ChatMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }
}
