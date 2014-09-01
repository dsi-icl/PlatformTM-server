using eTRIKS.Commons.Core.Domain.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace eTRIKS.Commons.Persistence.Models.Mapping
{
    public class ActivityMap : EntityTypeConfiguration<Activity>
    {
        public ActivityMap()
        {
            // Primary Key
            this.HasKey(t => t.OID);

            // Properties
            this.Property(t => t.OID)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Name)
                .HasMaxLength(2000);

            this.Property(t => t.StudyId)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Activity_TAB", "eTRIKSdata");
            this.Property(t => t.OID).HasColumnName("OID");
            this.Property(t => t.Name).HasColumnName("name");
            this.Property(t => t.StudyId).HasColumnName("studyId");

            // Relationships
            this.HasRequired(t => t.Study)
                .WithMany(s => s.Activities)
                .HasForeignKey(t => t.StudyId);

        }
    }
}
