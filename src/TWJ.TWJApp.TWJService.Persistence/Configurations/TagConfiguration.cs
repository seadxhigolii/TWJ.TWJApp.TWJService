using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Persistence.Configurations.Base;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations
{
    public class TagConfiguration : BaseEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            #region Configure Fields
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasColumnName("Id")
                .IsRequired();

            builder.Property(t => t.Name)
                .HasColumnName("Name")
                .IsRequired();

            builder.Property(t => t.PostCount)
                .HasColumnName("PostCount");
            #endregion

            #region Configure Table Name
            builder.ToTable("Tags");
            #endregion
        }
    }

}
