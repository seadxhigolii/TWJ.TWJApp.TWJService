using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities;
using TWJ.TWJApp.TWJService.Persistence.Configurations.Base;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations
{
    public class NewsConfiguration : BaseEntityTypeConfiguration<News>
    {
        public void Configure(EntityTypeBuilder<News> builder)
        {
            builder.HasKey(post => post.Id);
            builder.Property(post => post.Id).HasColumnName("Id").IsRequired();

            builder.Property(post => post.Title).HasColumnName("Title").IsRequired();
            builder.Property(post => post.Description).HasColumnName("Description").IsRequired();
            builder.Property(post => post.ReleaseDate).HasColumnName("ReleaseDate");
            builder.Property(post => post.URL).HasColumnName("URL");
            builder.Property(post => post.Tags).HasColumnName("Tags");
            builder.Property(post => post.IsUsed).HasColumnName("IsUsed");
            builder.Property(post => post.Active).HasColumnName("Active");
            builder.Property(post => post.NoOfPosts).HasColumnName("NoOfPosts");


            builder.ToTable("News");
        }
    }
}
