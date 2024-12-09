using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Persistence.Configurations.Base;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations
{
    public class TemplateConfiguration : BaseEntityTypeConfiguration<Template>
    {
        public void Configure(EntityTypeBuilder<Template> builder)
        {
            #region Properties

            builder.Property(q => q.Id).ValueGeneratedOnAdd().IsRequired();
            builder.Property(q => q.TemplateSettingId).IsRequired(true);
            builder.Property(q => q.DisplayText).HasMaxLength(250).IsRequired(true);
            builder.Property(q => q.Description).HasMaxLength(250).IsRequired(false);
            builder.Property(q => q.IsActive).IsRequired(true).HasDefaultValueSql("1");
            builder.Property(q => q.IsDefault).IsRequired(true).HasDefaultValueSql("0");
            builder.Property(q => q.ParentId).IsRequired(false);

            #endregion Properties

            #region Relations

            builder.HasOne(x => x.TemplateSetting)
                   .WithMany(x => x.TemplateList)
                   .HasForeignKey(x => x.TemplateSettingId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ParentTemplate)
                  .WithMany(x => x.TemplateList)
                  .HasForeignKey(x => x.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);

            #endregion Relations

            #region Table

            builder.ToTable("Template");

            #endregion Table
        }
    }
}
