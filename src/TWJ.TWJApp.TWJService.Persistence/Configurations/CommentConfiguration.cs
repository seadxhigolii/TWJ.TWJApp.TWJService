using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(comment => comment.Id);
            builder.Property(comment => comment.Id).HasColumnName("Id").IsRequired();

            builder.Property(comment => comment.Content).HasColumnName("Content").IsRequired();
            builder.Property(comment => comment.BlogPostID).HasColumnName("BlogPostID").IsRequired();
            builder.Property(comment => comment.UserID).HasColumnName("UserID").IsRequired();

            builder.HasOne(comment => comment.BlogPost)
                .WithMany(post => post.Comments)
                .HasForeignKey(comment => comment.BlogPostID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(comment => comment.User)
                .WithMany(user => user.Comments)
                .HasForeignKey(comment => comment.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Comments");
        }
    }
}
