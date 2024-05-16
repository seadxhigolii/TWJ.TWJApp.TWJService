using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Persistence.Configurations.Base;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations
{
    public class BlogPostConfiguration : BaseEntityTypeConfiguration<BlogPost>
    { 
        public void Configure(EntityTypeBuilder<BlogPost> builder)
        {
            builder.HasKey(post => post.Id);
            builder.Property(post => post.Id).HasColumnName("Id").IsRequired();

            builder.Property(post => post.Title).HasColumnName("Title").IsRequired();
            builder.Property(post => post.Content).HasColumnName("Content").IsRequired();
            builder.Property(post => post.SEOKeywords).HasColumnName("SEOKeywords");
            builder.Property(post => post.UserId).HasColumnName("UserId");
            builder.Property(post => post.ProductCategoryId).HasColumnName("ProductCategoryId").IsRequired();
            builder.Property(post => post.BlogPostCategoryId).HasColumnName("BlogPostCategoryId");
            builder.Property(post => post.Tags).HasColumnName("Tags");
            builder.Property(post => post.Image).HasColumnName("Image").HasColumnType("nvarchar(max)");
            builder.Property(post => post.Views).HasColumnName("Views");
            builder.Property(post => post.Likes).HasColumnName("Likes");
            builder.Property(post => post.Dislikes).HasColumnName("Dislikes");
            builder.Property(post => post.NumberOfComments).HasColumnName("NumberOfComments");
            builder.Property(post => post.ProductID).HasColumnName("ProductID");
            builder.Property(post => post.BackLinkKeywords).HasColumnName("BackLinkKeywords");
            builder.Property(post => post.URL).HasColumnName("URL");
            builder.Property(post => post.Published).HasColumnName("Published");

            builder.HasOne(post => post.Product)
                .WithOne()
                .HasForeignKey<BlogPost>(post => post.ProductID)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(post => post.Category)
                .WithMany(category => category.BlogPosts)
                .HasForeignKey(post => post.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(post => post.User)
                .WithMany(user => user.BlogPosts)
                .HasForeignKey(post => post.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("BlogPosts");
        }
    }
}
