using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            #region Configure Fields
            builder.HasKey(ur => new { ur.UserID, ur.RoleID });

            builder.Property(ur => ur.UserID)
                .HasColumnName("UserID")
                .IsRequired();

            builder.Property(ur => ur.RoleID)
                .HasColumnName("RoleID")
                .IsRequired();
            #endregion

            #region Configure Relationships
            builder.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleID)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Configure Table Name
            builder.ToTable("UserRoles");
            #endregion
        }
    }

}
