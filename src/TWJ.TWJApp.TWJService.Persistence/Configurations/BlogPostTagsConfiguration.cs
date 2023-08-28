using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations
{
    public class BlogPostTagsConfiguration : IEntityTypeConfiguration<BlogPostTags>
    {
        public void Configure(EntityTypeBuilder<BlogPostTags> builder)
        {
            builder.HasKey(postTag => postTag.Id);
            builder.Property(postTag => postTag.Id).HasColumnName("Id").IsRequired();

            builder.Property(postTag => postTag.BlogPostID).HasColumnName("BlogPostID").IsRequired();
            builder.Property(postTag => postTag.TagID).HasColumnName("TagID").IsRequired();

            builder.HasOne(postTag => postTag.Tag)
                .WithMany(tag => tag.BlogPostTags)
                .HasForeignKey(postTag => postTag.TagID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(postTag => postTag.BlogPost)
                .WithMany(post => post.BlogPostTags)
                .HasForeignKey(postTag => postTag.BlogPostID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("BlogPostTags");
        }
    }
}
