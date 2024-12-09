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
    public class CommentDislikeConfiguration : BaseEntityTypeConfiguration<CommentDislike>
    {
        public void Configure(EntityTypeBuilder<CommentDislike> builder)
        {
            #region Properties
            builder.HasKey(dislike => dislike.Id);
            builder.Property(dislike => dislike.Id).HasColumnName("Id").IsRequired();
            builder.Property(dislike => dislike.CommentID).HasColumnName("CommentID").IsRequired();
            builder.Property(dislike => dislike.UserID).HasColumnName("UserID").IsRequired();
            #endregion

            #region Relations
            builder.HasOne(dislike => dislike.User)
              .WithMany(user => user.CommentDislikes)
              .HasForeignKey(dislike => dislike.UserID)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dislike => dislike.Comment)
                .WithMany(comment => comment.Dislikes)
                .HasForeignKey(dislike => dislike.CommentID)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Table
            builder.ToTable("CommentDislikes");
            #endregion
        }
    }
}
