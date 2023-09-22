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
            builder.Property(post => post.UserId).HasColumnName("UserId").IsRequired();
            builder.Property(post => post.CategoryId).HasColumnName("CategoryId").IsRequired();
            builder.Property(post => post.Tags).HasColumnName("Tags");
            builder.Property(post => post.Image).HasColumnName("Image");
            builder.Property(post => post.Views).HasColumnName("Views").IsRequired();
            builder.Property(post => post.Likes).HasColumnName("Likes").IsRequired();
            builder.Property(post => post.Dislikes).HasColumnName("Dislikes").IsRequired();
            builder.Property(post => post.NumberOfComments).HasColumnName("NumberOfComments").IsRequired();
            builder.Property(post => post.ProductID).HasColumnName("ProductID");

            builder.HasOne(post => post.Product)
                .WithOne()
                .HasForeignKey<BlogPost>(post => post.ProductID)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(post => post.Category)
                .WithMany(category => category.BlogPosts)
                .HasForeignKey(post => post.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(post => post.User)
                .WithMany(user => user.BlogPosts)
                .HasForeignKey(post => post.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("BlogPosts");
        }
    }
}
