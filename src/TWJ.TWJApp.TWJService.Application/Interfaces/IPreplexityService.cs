using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Interfaces
{
    public interface IPreplexityService
    {
        Task<string[]> GenerateTitleAsync(string topic);
        Task<string> GenerateContentAsync(string topic);
    }
}
