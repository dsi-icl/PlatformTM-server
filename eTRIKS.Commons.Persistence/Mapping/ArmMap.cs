using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class ArmMap : EntityTypeConfiguration<Arm>
    {
        public ArmMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(2000);

            this.Property(t => t.Code)
                .IsRequired();
                //.HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Arm_TBL");
            this.Property(t => t.Id).HasColumnName("ArmId");
            this.Property(t => t.Name).HasColumnName("ArmName");
            this.Property(t => t.Code).HasColumnName("ArmCode");

            // Relationships
            //this.HasRequired(t => t.Project)
            //    .WithMany(s => s.Activities)
            //    .HasForeignKey(t => t.ProjectId);

            this.HasMany(t => t.Studies)
                .WithMany(t => t.Arms)
                .Map(mc =>
                {
                    mc.ToTable("Study_Arms");
                    mc.MapLeftKey("ArmId");
                    mc.MapRightKey("StudyId");

                });

        }
    }
}
