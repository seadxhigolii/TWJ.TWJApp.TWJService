using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TWJ.TWJApp.TWJService.Domain.Entities.Base;
using System;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations.Base
{
    public class BaseEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity<Guid>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("Id").IsRequired();

            builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");
            builder.Property(e => e.DeletedAt).HasColumnName("DeletedAt");
            builder.Property(e => e.CreatedBy).HasColumnName("CreatedBy");
        }

    }
}
