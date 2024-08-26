using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Interfaces.Scheduling
{
    public interface IScheduleService
    {
        Task<Dictionary<string, string>> GetUserSettingsAsync();
    }
}
