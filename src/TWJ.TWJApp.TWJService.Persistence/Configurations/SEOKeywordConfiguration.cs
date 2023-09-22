using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Persistence.Configurations.Base;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations
{
    public class SEOKeywordConfiguration : BaseEntityTypeConfiguration<SEOKeyword>
    {
        public void Configure(EntityTypeBuilder<SEOKeyword> builder)
        {
            #region Configure Fields
            builder.HasKey(k => k.Id);
            builder.Property(k => k.Id)
                .HasColumnName("Id")
                .IsRequired();

            builder.Property(k => k.Keyword)
                .HasColumnName("Keyword")
                .IsRequired();

            builder.Property(k => k.SearchVolume)
                .HasColumnName("SearchVolume")
                .IsRequired();

            builder.Property(k => k.CompetitionLevel)
                .HasColumnName("CompetitionLevel")
                .IsRequired();

            builder.Property(k => k.ClickThroughRate)
                .HasColumnName("ClickThroughRate")
                .IsRequired();
            #endregion

            #region Configure Table Name
            builder.ToTable("SEOKeywords");
            #endregion
        }
    }

}
