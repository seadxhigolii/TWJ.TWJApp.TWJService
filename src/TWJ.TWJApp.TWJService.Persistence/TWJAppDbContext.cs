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
                         .ForEach((x) => x.GetType());

            return base.SaveChangesAsync(cancellationToken);
        }
        public DbSet<User> User { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<BaseModel> Base { get; set; }
        public DbSet<TemplateSetting> TemplateSettings { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Category> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<BlogPostSEOKeyword> BlogPostSEOKeywords { get; set; }
        public DbSet<SEOKeyword> SEOKeywords { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<BlogPostCategory> BlogPostCategories { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<BlogPostTags> BlogPostTags { get; set; }
        public DbSet<NewsLetterSubscriber> NewsLetterSubscribers { get; set; }
        public DbSet<AdClick> AdClicks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());

            modelBuilder.Model.GetEntityTypes().ForEach(entityType =>
            {
                if (entityType.GetTableName() is string table && table.StartsWith("AspNet")) entityType.SetTableName(table[6..]);
            });

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TWJAppDbContext).Assembly);
        }
    }
}
