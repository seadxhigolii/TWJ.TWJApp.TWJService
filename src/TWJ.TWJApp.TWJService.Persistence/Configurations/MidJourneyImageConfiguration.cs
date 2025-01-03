﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Persistence.Configurations.Base;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations
{
    public class MidJourneyImageConfiguration : BaseEntityTypeConfiguration<MidJourneyImage>
    {
        public void Configure(EntityTypeBuilder<MidJourneyImage> builder)
        {
            builder.HasKey(image => image.Id);
            builder.Property(image => image.Id).HasColumnName("Id").IsRequired();

            builder.Property(image => image.Image).HasColumnName("Image").IsRequired();

            builder.ToTable("MidJourneyImages");
        }
    }
}
