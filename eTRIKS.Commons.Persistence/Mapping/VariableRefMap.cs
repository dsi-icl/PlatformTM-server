using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class VariableRefMap : EntityTypeConfiguration<VariableReference>
    {
        public VariableRefMap()
        {
            // Primary Key
            this.HasKey(t => new { t.VariableId, t.DatasetId });

            // Properties
            this.Property(t => t.VariableId)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.DatasetId)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Variable_Ref_TAB", "eTRIKSdata");
            this.Property(t => t.VariableId).HasColumnName("variableId");
            this.Property(t => t.DatasetId).HasColumnName("activityDatasetId");
            this.Property(t => t.OrderNumber).HasColumnName("orderNo");
            this.Property(t => t.IsRequired).HasColumnName("mandatory");
            this.Property(t => t.KeySequence).HasColumnName("keySequence");

            // Relationships
            this.HasRequired(t => t.Dataset)
                .WithMany(t => t.Variables)
                .HasForeignKey(d => d.DatasetId);
            this.HasRequired(t => t.Variable);
                //.WithMany(t => t.Variable_Ref_TAB)
                //.HasForeignKey(d => d.variableId);

        }
    }
}
