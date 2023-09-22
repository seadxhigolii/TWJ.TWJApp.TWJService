using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Persistence.Configurations.Base;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations
{
    public class TemplateSettingConfiguration : BaseEntityTypeConfiguration<TemplateSetting>
    {
        public void Configure(EntityTypeBuilder<TemplateSetting> builder)
        {
            #region Properties

            builder.Property(q => q.Id).ValueGeneratedOnAdd().IsRequired();
            builder.Property(q => q.DependOn).IsRequired(false);
            builder.Property(q => q.Name).HasMaxLength(250).IsRequired(true);
            builder.Property(q => q.Description).HasMaxLength(250).IsRequired(false);

            #endregion Properties

            #region Table

            builder.ToTable("TemplateSetting");

            #endregion Table
        }
    }
}
