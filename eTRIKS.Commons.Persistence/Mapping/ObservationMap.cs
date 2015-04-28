using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class ObservationMap : EntityTypeConfiguration<Observation>
    {
        public ObservationMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Table & Column Mappings
            this.ToTable("Observation_TBL");
            this.Property(t => t.Id).HasColumnName("ObservationId");
            //this.Property(t => t.Name).HasColumnName("Name");
            //this.Property(t => t.StudyId).HasColumnName("StudyId");

            // Relationships
            this.HasOptional(t => t.ControlledTerm)
                .WithMany()
                .HasForeignKey(t => t.ControlledTermId);

            this.HasRequired(t => t.TopicVariable)
                .WithMany()
                .HasForeignKey(t => t.TopicVariableId);

            this.HasRequired(t => t.DefaultQualifier)
                .WithMany()
                .HasForeignKey(t => t.DefaultQualifierId);

            this.HasMany(t => t.Qualifiers)
                .WithMany()
                .Map(mc =>
                {
                    mc.ToTable("Observation_Qualfiers");
                    mc.MapLeftKey("ObservationId");
                    mc.MapRightKey("QualifierId");
                    
                });

            this.HasMany(t => t.Timings)
                .WithMany()
                .Map(mc =>
                {
                    mc.ToTable("Observation_Timings");
                    mc.MapLeftKey("ObservationId");
                    mc.MapRightKey("QualifierId");

                });

            this.HasMany(t => t.Synonyms)
                .WithMany()
                .Map(mc =>
                {
                    mc.ToTable("Observation_Synonyms");
                    mc.MapLeftKey("ObservationId");
                    mc.MapRightKey("QualifierId");

                });

        }
    }
}
