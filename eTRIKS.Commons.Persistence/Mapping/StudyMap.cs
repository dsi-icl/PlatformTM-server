using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class StudyMap : EntityTypeConfiguration<Study>
    {
        public StudyMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Name)
                .HasMaxLength(2000);

            this.Property(t => t.Description)
                .HasMaxLength(2000);

            // Table & Column Mappings
            this.ToTable("Study_TBL");
            this.Property(t => t.Id).HasColumnName("OID");

            // Relationships
            this.HasRequired(t => t.Project)
                .WithMany(s => s.Studies)
                .HasForeignKey(t => t.ProjectId);


            this.HasMany(t => t.Observations)
                .WithMany(t => t.Studies)
                .Map(mc =>
                {
                    mc.ToTable("Study_Observations");
                    mc.MapLeftKey("StudyId");
                    mc.MapRightKey("ObservationId");

                });
        }
    }
}
