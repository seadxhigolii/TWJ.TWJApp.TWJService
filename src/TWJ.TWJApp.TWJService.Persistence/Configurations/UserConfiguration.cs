using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Persistence.Configurations.Base;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations
{
    public class UserConfiguration : BaseEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            #region Configure Fields
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnName("Id").IsRequired();

            builder.Property(u => u.FirstName).HasColumnName("FirstName").IsRequired();
            builder.Property(u => u.LastName).HasColumnName("LastName").IsRequired();
            builder.Property(u => u.UserName).HasColumnName("UserName").IsRequired();
            builder.Property(u => u.Image).HasColumnName("Image").IsRequired();
            builder.Property(u => u.Password).HasColumnName("Password").IsRequired();
            builder.Property(u => u.Email).HasColumnName("Email").IsRequired();
            builder.Property(u => u.EmailConfirmed).HasColumnName("EmailConfirmed").IsRequired();
            builder.Property(u => u.isActive).HasColumnName("isActive").IsRequired();
            builder.Property(u => u.DateOfBirth).HasColumnName("DateOfBirth").IsRequired();
            builder.Property(u => u.City).HasColumnName("City").IsRequired();
            builder.Property(u => u.Country).HasColumnName("Country").IsRequired();
            builder.Property(u => u.Description).HasColumnName("Description");
            #endregion

            #region Configure Table Name
            builder.ToTable("Users");
            #endregion

            #region Relationship Configuration
            builder.HasMany(u => u.BlogPosts)
                .WithOne(p => p.User) 
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict); 
            #endregion
        }
    }
}
