using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Extensions;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;
using TWJ.TWJApp.TWJService.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Persistence.Configurations;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Persistence
{
    public class TWJAppDbContext : DbContext, ITWJAppDbContext
    {
        public TWJAppDbContext(DbContextOptions<TWJAppDbContext> options) : base(options) { }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.Entries()
                         .Where(x => typeof(IEntityTimeStamp).IsAssignableFrom(x.Entity.GetType()) && x.State == EntityState.Modified)
                         .Select(x => x.Entity)
                         .ForEach((x) => x.GetType().GetProperty(CommonEntityConfigModel.UpdatedAt).SetValue(x, DateTime.UtcNow));

            return base.SaveChangesAsync(cancellationToken);
        }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());

            modelBuilder.Model.GetEntityTypes().ForEach(entityType =>
            {
                modelBuilder.Entity(entityType.ClrType, builder => builder.UseBaseConfigurations(entityType.ClrType));

                if (entityType.GetTableName() is string table && table.StartsWith("AspNet")) entityType.SetTableName(table[6..]);
            });

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TWJAppDbContext).Assembly);
        }
    }
}
