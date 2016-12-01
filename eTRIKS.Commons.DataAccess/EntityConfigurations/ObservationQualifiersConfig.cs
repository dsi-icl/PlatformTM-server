using eTRIKS.Commons.Core.JoinEntities;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class ObservationQualifiersConfig : EntityTypeConfiguration<ObservationQualifier>
    {
        public override void Configure(EntityTypeBuilder<ObservationQualifier> builder)
        {
            builder
                .HasKey(t => new { t.ObservationId, t.QualifierId });

            builder
               .ToTable("Observation_Qualfiers");

            builder
                .HasOne(dd => dd.Qualifier)
                .WithMany()
                .HasForeignKey(dd => dd.QualifierId);

            builder
                .HasOne(dd => dd.Observation)
                .WithMany(o=>o.Qualifiers)
                .HasForeignKey(dd => dd.ObservationId);
                
        }
    }
}
