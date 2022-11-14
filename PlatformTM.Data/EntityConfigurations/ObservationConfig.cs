using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{
    //public class ObservationConfig : EntityTypeConfiguration<Observation>
    //{
    //    public override void Configure(EntityTypeBuilder<Observation> builder)
    //    {
    //        // Primary Key
    //        builder.HasKey(t => t.Id);

    //        // Table & Column Mappings
    //        builder.ToTable("Observations");
    //        builder.Property(t => t.Id).HasColumnName("ObservationId");

    //        // Relationships
    //        builder.HasOne(t => t.ControlledTerm)
    //            .WithMany()
    //            .HasForeignKey(t => t.ControlledTermId);

    //        builder.HasOne(t => t.TopicVariable)
    //            .WithMany()
    //            .IsRequired()
    //            .HasForeignKey(t => t.TopicVariableId);

    //        builder
    //            .HasOne(t => t.DefaultQualifier)
    //            .WithMany()
    //            .IsRequired()
    //            .HasForeignKey(t => t.DefaultQualifierId);
    //    }
    //}
}
