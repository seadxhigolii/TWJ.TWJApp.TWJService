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
    public class BlogPostSEOKeywordConfiguration : IEntityTypeConfiguration<BlogPostSEOKeyword>
    {
        public void Configure(EntityTypeBuilder<BlogPostSEOKeyword> builder)
        {
            builder.HasKey(postKeyword => new { postKeyword.BlogPostID, postKeyword.SEOKeywordID });

            builder.Property(postKeyword => postKeyword.BlogPostID)
                .HasColumnName("BlogPostID")
                .IsRequired();

            builder.Property(postKeyword => postKeyword.SEOKeywordID)
                .HasColumnName("SEOKeywordID")
                .IsRequired();

            builder.HasOne(postKeyword => postKeyword.BlogPost)
                .WithMany(post => post.BlogPostSEOKeywords)
                .HasForeignKey(postKeyword => postKeyword.BlogPostID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(postKeyword => postKeyword.SEOKeyword)
                .WithMany(keyword => keyword.BlogPostSEOKeywords)
                .HasForeignKey(postKeyword => postKeyword.SEOKeywordID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("BlogPostSEOKeywords");
        }
    }
}
