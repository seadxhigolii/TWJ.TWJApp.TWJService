using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Application.Interfaces
{
    public interface ITWJAppDbContext
    {
        DbSet<User> User { get; set; }
        DbSet<Template> Template { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
