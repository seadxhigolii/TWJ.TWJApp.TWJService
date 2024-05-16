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
    public class AdClickConfiguration : BaseEntityTypeConfiguration<AdClick>
    {
        public void Configure(EntityTypeBuilder<AdClick> builder)
        {
            builder.HasKey(ac => ac.Id);
            builder.Property(ac => ac.Id).HasColumnName("Id").IsRequired();

            builder.Property(ac => ac.BlogPostId).IsRequired();
            builder.Property(ac => ac.ProductId);

            builder.Property(ac => ac.ClickTime);
            builder.Property(ac => ac.UserSessionId);
            builder.Property(ac => ac.Converted);

            builder.HasOne(ac => ac.Product)
                .WithMany(p => p.AdClicks)
                .HasForeignKey(ac => ac.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ac => ac.BlogPost)
                .WithMany(bp => bp.AdClicks)
                .HasForeignKey(ac => ac.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("AdClicks");
        }
    }
}
