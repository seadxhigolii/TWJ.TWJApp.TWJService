using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Persistence.Configurations.Base;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations
{
    public class InstagramPostConfiguration : BaseEntityTypeConfiguration<InstagramPost>
    {
        public void Configure(EntityTypeBuilder<InstagramPost> builder)
        {
            builder.HasKey(post => post.Id);
            builder.Property(post => post.Id).HasColumnName("Id").IsRequired();

            builder.Property(post => post.Image).HasColumnName("Image").IsRequired();

            builder.ToTable("InstagramPosts");
        }
    }
}
