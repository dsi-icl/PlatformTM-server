using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model.Templates;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class DomainVariableMap : EntityTypeConfiguration<DomainTemplateVariable>
    {
        public DomainVariableMap()
        {
            // Primary Key
            this.HasKey(t => t.OID);

            // Properties
            this.Property(t => t.OID)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Name)
                .HasMaxLength(2000);

            this.Property(t => t.Description)
                .HasMaxLength(2000);

            this.Property(t => t.DataType)
                .HasMaxLength(200);

            this.Property(t => t.DomainId)
                .HasMaxLength(200);

            this.Property(t => t.VariableType)
                .HasMaxLength(200);

            this.Property(t => t.RoleId)
                .HasMaxLength(200);

            this.Property(t => t.Label)
                .HasMaxLength(2000);

            this.Property(t => t.UsageId)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Variable_Template_TAB", "eTRIKSdata");
            this.Property(t => t.OID).HasColumnName("OID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.DataType).HasColumnName("DataType");
            this.Property(t => t.DomainId).HasColumnName("DomainId");
            this.Property(t => t.VariableType).HasColumnName("VariableType");
            this.Property(t => t.RoleId).HasColumnName("Role");
            this.Property(t => t.Label).HasColumnName("Label");
            this.Property(t => t.UsageId).HasColumnName("Usage");

            // Relationships
            this.HasOptional(t => t.Domain)
                .WithMany(t => t.Variables)
                .HasForeignKey(d => d.DomainId);

        }
    }
}
