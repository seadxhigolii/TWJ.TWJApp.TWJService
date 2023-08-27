using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
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
            #endregion

            #region Configure Table Name
            builder.ToTable("Tags");
            #endregion
        }
    }

}
