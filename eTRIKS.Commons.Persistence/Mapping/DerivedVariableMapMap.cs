using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace eTRIKS.Commons.Persistence.Models.Mapping
{
    public class Derived_Variable_Map_TABMap : EntityTypeConfiguration<Derived_Variable_Map_TAB>
    {
        public Derived_Variable_Map_TABMap()
        {
            // Primary Key
            this.HasKey(t => new { t.variableId, t.sourceVariableId });

            // Properties
            this.Property(t => t.variableId)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.sourceVariableId)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.temp)
                .HasMaxLength(45);

            // Table & Column Mappings
            this.ToTable("Derived_Variable_Map_TAB", "eTRIKS_schema");
            this.Property(t => t.variableId).HasColumnName("variableId");
            this.Property(t => t.sourceVariableId).HasColumnName("sourceVariableId");
            this.Property(t => t.temp).HasColumnName("temp");

            // Relationships
            this.HasRequired(t => t.Variable_Def_TAB)
                .WithMany(t => t.Derived_Variable_Map_TAB)
                .HasForeignKey(d => d.variableId);
            this.HasRequired(t => t.Variable_Def_TAB1)
                .WithMany(t => t.Derived_Variable_Map_TAB1)
                .HasForeignKey(d => d.sourceVariableId);

        }
    }
}
