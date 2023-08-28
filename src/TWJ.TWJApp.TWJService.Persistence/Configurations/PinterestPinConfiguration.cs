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
    public class PinterestPinConfiguration : IEntityTypeConfiguration<PinterestPin>
    {
        public void Configure(EntityTypeBuilder<PinterestPin> builder)
        {
            builder.HasKey(pin => pin.Id);
            builder.Property(pin => pin.Id).HasColumnName("Id").IsRequired();

            builder.Property(pin => pin.Image).HasColumnName("Image").IsRequired();

            builder.ToTable("PinterestPins");
        }
    }
}
