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
        DbSet<Template> Templates { get; set; }
        DbSet<BaseModel> Base { get; set; }
        DbSet<BlogPost> BlogPosts { get; set; }
        DbSet<BlogPostCategory> BlogPostCategories { get; set; }
        DbSet<TemplateSetting> TemplateSettings { get; set; }
        DbSet<Category> ProductCategories { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<BlogPostSEOKeyword> BlogPostSEOKeywords { get; set; }
        DbSet<SEOKeyword> SEOKeywords { get; set; }
        DbSet<Log> Logs { get; set; }
        DbSet<News> News { get; set; }
        DbSet<Tag> Tag { get; set; }
        DbSet<BlogPostTags> BlogPostTags { get; set; }
        DbSet<NewsLetterSubscriber> NewsLetterSubscribers { get; set; }
        DbSet<Banner> Banners { get; set; }
        DbSet<BlogPostBanner> BlogPostBanners { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
