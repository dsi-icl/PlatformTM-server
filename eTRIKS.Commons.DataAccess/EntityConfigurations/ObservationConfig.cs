using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;
using eTRIKS.Commons.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class ObservationConfig : EntityTypeConfiguration<Observation>
    {
        public override void Configure(EntityTypeBuilder<Observation> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            // Table & Column Mappings
            builder.ToTable("Observations");
            builder.Property(t => t.Id).HasColumnName("ObservationId");

            // Relationships
            builder.HasOne(t => t.ControlledTerm)
                .WithOne()
            .HasForeignKey<Observation>(t => t.ControlledTermId);

            builder.HasOne(t => t.TopicVariable)
                .WithOne()
                .IsRequired()
            .HasForeignKey<Observation>(t => t.TopicVariableId);

            builder
                .HasOne(t => t.DefaultQualifier)
                .WithOne()
                .IsRequired()
                .HasForeignKey<Observation>(t => t.DefaultQualifierId);
        }
    }
}
