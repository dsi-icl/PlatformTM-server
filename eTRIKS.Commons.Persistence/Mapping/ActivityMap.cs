using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class ActivityMap : EntityTypeConfiguration<Activity>
    {
        public ActivityMap()
        {
            // Primary Key
            this.HasKey(t => t.OID);

            // Properties
            //this.Property(t => t.OID)
            //    .IsRequired()
            //    .HasMaxLength(200);

            this.Property(t => t.Name)
                .HasMaxLength(2000);

            this.Property(t => t.StudyId)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Activity_TBL");
            this.Property(t => t.OID).HasColumnName("ActivityId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.StudyId).HasColumnName("StudyId");

            // Relationships
            this.HasRequired(t => t.Study)
                .WithMany(s => s.Activities)
                .HasForeignKey(t => t.StudyId);

        }
    }
}
