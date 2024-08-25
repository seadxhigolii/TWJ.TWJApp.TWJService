using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Interfaces.Scheduling;

namespace TWJ.TWJApp.TWJService.Application.Services.Scheduling
{
    public class ScheduleService : IScheduleService
    {
        private readonly ITWJAppDbContext _context;

        public ScheduleService(ITWJAppDbContext context)
        {
            _context = context;
        }
        public async Task<Dictionary<string, string>> GetUserSettingsAsync()
        {
            var settings = await _context.UserSettings.ToListAsync();
            return settings.ToDictionary(s => s.Key, s => s.Value);
        }
    }
}
