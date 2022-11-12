using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.Domain.Model.DesignElements;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{
    public class CohortConfig : EntityTypeConfiguration<Cohort>
    {
        public override void Configure(EntityTypeBuilder<Cohort> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(2000);

            builder
                .Property(t => t.Code)
                .IsRequired();
            //.HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("Cohorts");
            builder.Property(t => t.Id).HasColumnName("CohortId");
            builder.Property(t => t.Name).HasColumnName("CohortName");
            builder.Property(t => t.Code).HasColumnName("CohortCode");

            //CONSIDER WHEN M-2-M realtionships are supported again

            //builder.HasMany(t => t.Studies)
            //    .WithMany(t => t.Arms)
            //    .Map(mc =>
            //    {
            //        mc.ToTable("Study_Arms");
            //        mc.MapLeftKey("ArmId");
            //        mc.MapRightKey("StudyId");

            //    });
        }
    }
}
