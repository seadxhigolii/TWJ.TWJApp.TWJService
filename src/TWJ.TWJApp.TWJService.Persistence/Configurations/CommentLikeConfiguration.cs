using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Persistence.Configurations.Base;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations
{
    public class CommentLikeConfiguration : BaseEntityTypeConfiguration<CommentLike>
    {
        public void Configure(EntityTypeBuilder<CommentLike> builder)
        {
            builder.HasKey(like => like.Id);
            builder.Property(like => like.Id).HasColumnName("Id").IsRequired();

            builder.Property(like => like.CommentID).HasColumnName("CommentID").IsRequired();
            builder.Property(like => like.UserID).HasColumnName("UserID").IsRequired();

            builder.HasOne(like => like.User)
                .WithMany(user => user.CommentLikes)
                .HasForeignKey(like => like.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(like => like.Comment)
                .WithMany(comment => comment.Likes)
                .HasForeignKey(like => like.CommentID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("CommentLikes");
        }
    }
}
