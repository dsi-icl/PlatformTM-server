using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class ActivityMap : EntityTypeConfiguration<Activity>
    {
        public ActivityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            this.Property(t => t.Name)
                .HasMaxLength(2000);

            this.Property(t => t.ProjectId)
                .IsRequired();
                //.HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Activity_TBL");
            this.Property(t => t.Id).HasColumnName("ActivityId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ProjectId).HasColumnName("ProjectId");

            // Relationships
            this.HasRequired(t => t.Project)
                .WithMany(s => s.Activities)
                .HasForeignKey(t => t.ProjectId);

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
