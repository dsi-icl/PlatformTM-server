using System;
using PlatformTM.Core.Domain.Model.BMO;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace PlatformTM.Data.EntityConfigurations
{

    internal class ObservationPropertyConfig : EntityTypeConfiguration<ObservationProperty>
    {
        public override void Configure(EntityTypeBuilder<ObservationProperty> builder)
        {
            //key  
            builder.HasKey(t => t.Id);

            //property  
            builder.Property(t => t.Id); ;

            // Relationships


            //table  
            builder.ToTable("ObservationProperties");
        }
    }
}
