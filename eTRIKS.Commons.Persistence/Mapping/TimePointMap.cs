using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Timing;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class TimePointMap : EntityTypeConfiguration<TimePoint>
    {
        public TimePointMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            this.Property(t => t.Name)
                .HasMaxLength(2000);

            this.Property(t => t.Number);

            // Table & Column Mappings
            this.ToTable("TimePoints_TBL");
            this.Property(t => t.Id).HasColumnName("TimePointId");
            //this.Property(t => t.Name).HasColumnName("Name");
            //this.Property(t => t.StudyId).HasColumnName("StudyId");

            // Relationships
            //this.HasRequired(t => t.Study)
            //    .WithMany(s => s.Activities)
            //    .HasForeignKey(t => t.StudyId);

            //this.HasMany(t => t.Studies)
            //    .WithMany(t => t.Activities)
            //    .Map(mc =>
            //    {
            //        mc.ToTable("Study_Activities");
            //        mc.MapLeftKey("StudyId");
            //        mc.MapRightKey("ActivityId");

            //    });

        }
    }
}
