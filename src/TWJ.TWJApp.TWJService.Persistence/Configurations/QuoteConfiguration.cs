using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Persistence.Configurations.Base;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations
{
    public class QuoteConfiguration : BaseEntityTypeConfiguration<Quote>
    {
        public void Configure(EntityTypeBuilder<Quote> builder)
        {
            builder.HasKey(ac => ac.Id);
            builder.Property(ac => ac.Id).HasColumnName("Id").IsRequired();

            builder.Property(ac => ac.Content);
            builder.Property(ac => ac.Author);
            builder.Property(ac => ac.Category);
            builder.Property(ac => ac.Length);

            builder.HasIndex(ac => ac.Content).IsUnique();

            builder.ToTable("Quotes");
        }
    }
}
