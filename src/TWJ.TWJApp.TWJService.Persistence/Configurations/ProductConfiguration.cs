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
    public class ProductConfiguration : BaseEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("Id").IsRequired();

            builder.Property(p => p.ProductName).HasColumnName("ProductName").IsRequired();
            builder.Property(p => p.Description).HasColumnName("Description");
            builder.Property(p => p.VendorName).HasColumnName("VendorName");
            builder.Property(p => p.CategoryId).HasColumnName("CategoryId").IsRequired();
            builder.Property(p => p.AvgRating).HasColumnName("AvgRating");
            builder.Property(p => p.TotalRatings).HasColumnName("TotalRatings");
            builder.Property(p => p.Price).HasColumnName("Price");
            builder.Property(p => p.Currency).HasColumnName("Currency");
            builder.Property(p => p.AffiliateLink).HasColumnName("AffiliateLink");
            builder.Property(p => p.Image).HasColumnName("Image");
            builder.Property(p => p.PromotionStart).HasColumnName("PromotionStart");
            builder.Property(p => p.PromotionEnd).HasColumnName("PromotionEnd");
            builder.Property(p => p.Active).HasColumnName("Active");

            builder.HasOne(p => p.Category).WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Products");
        }
    }

}
