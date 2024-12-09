using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Persistence.Configurations.Base;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations
{
    public class CommentReplyConfiguration : BaseEntityTypeConfiguration<CommentReply>
    {
        public void Configure(EntityTypeBuilder<CommentReply> builder)
        {
            builder.HasKey(reply => reply.Id);
            builder.Property(reply => reply.Id).HasColumnName("Id").IsRequired();

            builder.Property(reply => reply.Content).HasColumnName("Content").IsRequired();
            builder.Property(reply => reply.CommentID).HasColumnName("CommentID").IsRequired();
            builder.Property(reply => reply.UserID).HasColumnName("UserID").IsRequired();

            builder.HasOne(reply => reply.User)
                .WithMany(user => user.CommentReplies)
                .HasForeignKey(reply => reply.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(reply => reply.Comment)
                .WithMany(comment => comment.Replies)
                .HasForeignKey(reply => reply.CommentID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("CommentReplies");
        }
    }
}
