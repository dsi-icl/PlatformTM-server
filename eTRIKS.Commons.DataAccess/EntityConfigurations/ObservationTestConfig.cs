using System;
using System.Collections.Generic;
using System.Text;
using eTRIKS.Commons.Core.Domain.Model.ObservationModel;
using eTRIKS.Commons.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Observation = eTRIKS.Commons.Core.Domain.Model.Observation;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class ObservationTestConfig : EntityTypeConfiguration<ObservationTest>
    {
        public override void Configure(EntityTypeBuilder<ObservationTest> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id); 

            // Table & Column Mappings
            builder.ToTable("ObservationsTest");
            builder.Property(t => t.Id).HasColumnName("ObservationId");

            // Relationships
            builder.HasOne(t => t.ObservedValue)
                .WithOne()
                .HasForeignKey<ObservationTest>(t => t.ObservedValue);

            builder.HasOne(t => t.TemporalValue)
                .WithOne()
                .IsRequired()
                .HasForeignKey<ObservationTest>(t => t.TemporalValue);

            builder
                .HasOne(t => t.TimeSeriesValue)
                .WithOne()
                .IsRequired()
                .HasForeignKey<ObservationTest>(t => t.TimeSeriesValue);
        }
    }
}
