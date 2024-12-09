using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Persistence.Configurations.Base;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations
{
    public class BlogPostCategoryConfiguration : BaseEntityTypeConfiguration<BlogPostCategory>
    {
        public void Configure(EntityTypeBuilder<BlogPostCategory> builder)
        {
            builder.HasKey(category => category.Id);
            builder.Property(category => category.Id).HasColumnName("Id").IsRequired();

            builder.Property(category => category.Name).HasColumnName("Name").IsRequired();
            builder.Property(category => category.Description).HasColumnName("Description");
            builder.Property(category => category.URL).HasColumnName("URL");

            builder.ToTable("BlogPostCategories");
        }
    }
}
