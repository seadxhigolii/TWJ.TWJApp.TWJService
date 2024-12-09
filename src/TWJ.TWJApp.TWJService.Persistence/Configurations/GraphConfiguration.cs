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
    public class GraphConfiguration : BaseEntityTypeConfiguration<Graph>
    {
        public void Configure(EntityTypeBuilder<Graph> builder)
        {
            builder.HasKey(ac => ac.Id);
            builder.Property(ac => ac.Id).HasColumnName("Id").IsRequired();

            builder.Property(ac => ac.Title);
            builder.Property(ac => ac.IsUsed);

            builder.ToTable("Graphs");
        }
    }
}
