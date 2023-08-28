using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations
{
    public class PinterestKeywordsConfiguration : IEntityTypeConfiguration<PinterestPinKeywords>
    {
        public void Configure(EntityTypeBuilder<PinterestPinKeywords> builder)
        {
            #region Configure Fields
            builder.HasKey(pk => pk.Id);
            builder.Property(pk => pk.Id)
                .HasColumnName("Id")
                .IsRequired();

            builder.Property(pk => pk.PinterestPinId)
                .HasColumnName("PinterestPinId")
                .IsRequired();

            builder.Property(pk => pk.Keyword)
                .HasColumnName("Keyword")
                .IsRequired();
            #endregion

            #region Configure Relationships
            builder.HasOne(pk => pk.PinterestPin)
                .WithMany(p => p.PinterestPinKeywords)
                .HasForeignKey(pk => pk.PinterestPinId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Configure Table Name
            builder.ToTable("PinterestPinKeywords");
            #endregion
        }
    }
}
