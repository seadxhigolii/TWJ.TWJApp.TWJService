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
    public class BannerConfiguration : BaseEntityTypeConfiguration<Banner>
    {
        public void Configure(EntityTypeBuilder<Banner> builder)
        {
            builder.HasKey(like => like.Id);
            builder.Property(like => like.Id).HasColumnName("Id").IsRequired();

            builder.Property(like => like.AdvertiserName).HasColumnName("AdvertiserName").IsRequired();
            builder.Property(like => like.ImageUrl).HasColumnName("ImageUrl").IsRequired();
            builder.Property(like => like.ProductId).HasColumnName("ProductId");
            builder.Property(like => like.DestinationUrl).HasColumnName("DestinationUrl");
            builder.Property(like => like.AltText).HasColumnName("AltText");
            builder.Property(like => like.Position).HasColumnName("Position");
            builder.Property(like => like.Clicks).HasColumnName("Clicks");
            builder.Property(like => like.TimesShown).HasColumnName("TimesShown");
            builder.Property(like => like.CTR).HasColumnName("CTR");
            builder.Property(like => like.Width).HasColumnName("Width");
            builder.Property(like => like.StartDate).HasColumnName("StartDate");
            builder.Property(like => like.EndDate).HasColumnName("EndDate");
            builder.Property(like => like.Active).HasColumnName("Active");

            builder.HasOne(banner => banner.Product)
                   .WithMany(product => product.Banners)
                   .HasForeignKey(banner => banner.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Banners");
        }
    }
}
