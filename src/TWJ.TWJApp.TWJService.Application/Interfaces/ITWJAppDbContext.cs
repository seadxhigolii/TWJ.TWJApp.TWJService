using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;

namespace TWJ.TWJApp.TWJService.Application.Interfaces
{
    public interface ITWJAppDbContext
    {
        DbSet<User> User { get; set; }
        DbSet<Template> Template { get; set; }
        DbSet<BaseModel> Base { get; set; }
        DbSet<TemplateSetting> TemplateSetting { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
