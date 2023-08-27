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
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("Id").IsRequired();

            builder.Property(p => p.ProductName).HasColumnName("ProductName").IsRequired();
            builder.Property(p => p.Description).HasColumnName("Description").IsRequired();
            builder.Property(p => p.VendorName).HasColumnName("VendorName").IsRequired();
            builder.Property(p => p.CategoryId).HasColumnName("CategoryId").IsRequired();
            builder.Property(p => p.AvgRating).HasColumnName("AvgRating");
            builder.Property(p => p.TotalRatings).HasColumnName("TotalRatings");
            builder.Property(p => p.Price).HasColumnName("Price").IsRequired();
            builder.Property(p => p.Currency).HasColumnName("Currency");
            builder.Property(p => p.AffiliateLink).HasColumnName("AffiliateLink").IsRequired();
            builder.Property(p => p.Image).HasColumnName("Image").IsRequired();
            builder.Property(p => p.PromotionStart).HasColumnName("PromotionStart").IsRequired();
            builder.Property(p => p.PromotionEnd).HasColumnName("PromotionEnd").IsRequired();

            builder.HasOne(p => p.Category).WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Products");
        }
    }

}
