using TWJ.TWJApp.TWJService.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using static TWJ.TWJApp.TWJService.Persistence.Configurations.Base.CommonEntityConfigModel;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations.Base
{
    public static class BaseEntityConfiguration
    {
        public static void UseBaseConfigurations(this EntityTypeBuilder builder, Type type)
        {
            GetBaseConfigModels().ForEach(value =>
            {
                if (type.HasProperty(value.Property))
                {
                    if (value.IsKey)
                        builder.HasKey(value.Property);

                    builder.Property(value.Property)
                        .HasColumnName(value.Value)
                        .IsRequired(value.IsRequired);
                }
            });
        }
    }
}
