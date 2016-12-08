using eTRIKS.Commons.Core.JoinEntities;
using eTRIKS.Commons.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class ObservationTimingsConfig : EntityTypeConfiguration<ObservationTiming>
    {
        public override void Configure(EntityTypeBuilder<ObservationTiming> builder)
        {
            builder
                .HasKey(t => new { t.ObservationId, t.QualifierId });

            builder
               .ToTable("Observation_Timings");

            builder
                .HasOne(dd => dd.Qualifier)
                .WithMany()
                .HasForeignKey(dd => dd.QualifierId);

            builder
                .HasOne(dd => dd.Observation)
                .WithMany(o=>o.Timings)
                .HasForeignKey(dd => dd.ObservationId);
        }
    }
}
