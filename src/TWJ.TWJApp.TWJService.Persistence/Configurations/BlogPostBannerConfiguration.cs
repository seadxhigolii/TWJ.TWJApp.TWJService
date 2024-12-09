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
    public class BlogPostBannerConfiguration : IEntityTypeConfiguration<BlogPostBanner>
    {
        public void Configure(EntityTypeBuilder<BlogPostBanner> builder)
        {
            builder.HasKey(bpBanner => bpBanner.Id);
            builder.Property(bpBanner => bpBanner.Id).HasColumnName("Id").IsRequired();

            builder.Property(bpBanner => bpBanner.BlogPostId).HasColumnName("BlogPostId").IsRequired();
            builder.Property(bpBanner => bpBanner.BannerId).HasColumnName("BannerId").IsRequired();

            builder.HasOne(bpBanner => bpBanner.BlogPost)
                .WithMany(blogPost => blogPost.BlogPostBanners)
                .HasForeignKey(bpBanner => bpBanner.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bpBanner => bpBanner.Banner)
                .WithMany(banner => banner.BlogPostBanners)
                .HasForeignKey(bpBanner => bpBanner.BannerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("BlogPostBanner");
        }
    }
}
