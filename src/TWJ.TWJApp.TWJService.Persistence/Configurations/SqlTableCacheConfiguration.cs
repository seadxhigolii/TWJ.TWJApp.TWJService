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
    public class SqlTableCacheConfiguration : BaseEntityTypeConfiguration<SqlTableCache>
    {
        public void Configure(EntityTypeBuilder<SqlTableCache> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasMaxLength(449).IsRequired();
            builder.Property(c => c.Value).IsRequired(true);
            builder.Property(c => c.ExpiresAtTime).IsRequired(true);
            builder.Property(c => c.SlidingExpirationInSeconds).IsRequired(false);
            builder.Property(c => c.AbsoluteExpiration).IsRequired(false);

            builder.ToTable("SqlTableCache");
        }
    }
}
