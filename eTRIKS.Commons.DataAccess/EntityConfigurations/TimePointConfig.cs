using eTRIKS.Commons.Core.Domain.Model.DesignElements;
using eTRIKS.Commons.Core.Domain.Model.Timing;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class TimePointConfig : EntityTypeConfiguration<TimePoint>
    {
        public override void Configure(EntityTypeBuilder<TimePoint> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .HasMaxLength(2000);

            //builder.Property(t => t.StudyId)
            //    .IsRequired();
            //.HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("Timepoints");
            builder
             .HasDiscriminator<string>("Discriminator")
            .HasValue<AbsoluteTimePoint>("AbsoluteTimePoint")
            .HasValue<RelativeTimePoint>("RelativeTimePoint");


            // Relationships
            //builder.HasOne(t => t.)
            //    .WithMany(s => s.Visits)
            //    .IsRequired()
            //    .HasForeignKey(t => t.StudyId);

            //this.HasMany(t => t.TimePoints)
            //    .WithMany()
            //    .Map(mc =>
            //    {
            //        mc.ToTable("Visit_TimePoints");
            //        mc.MapLeftKey("VisitId");
            //        mc.MapRightKey("TimePointId");

            //    });
        }
    }
}
