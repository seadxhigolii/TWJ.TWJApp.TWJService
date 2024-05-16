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
    public class NewsletterSubscriberConfiguration : BaseEntityTypeConfiguration<NewsLetterSubscriber>
    {
        public void Configure(EntityTypeBuilder<NewsLetterSubscriber> builder)
        {
            #region Configure Fields
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasColumnName("Id")
                .IsRequired();

            builder.Property(t => t.Email)
                .HasColumnName("Email")
                .IsRequired();

            builder.Property(t => t.Subscribed)
                .HasColumnName("Subscribed")
                .IsRequired();

            builder.Property(t => t.DateUnsubscribed)
                .HasColumnName("DateUnsubscribed");
            #endregion

            #region Configure Table Name
            builder.ToTable("NewsletterSubscribers");
            #endregion
        }
    }
}
