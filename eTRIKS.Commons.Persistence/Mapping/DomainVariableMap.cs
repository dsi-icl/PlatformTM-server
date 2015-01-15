using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model.Templates;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class DomainVariableMap : EntityTypeConfiguration<DomainVariableTemplate>
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

            this.Property(t => t.Label)
                .HasMaxLength(2000);

            this.Property(t => t.DomainId)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.VariableTypeId)
                .HasMaxLength(200);

            this.Property(t => t.RoleId)
                .HasMaxLength(200);
            
            this.Property(t => t.UsageId)
                .HasMaxLength(200);

            this.Property(t => t.controlledTerminologyId)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Templates.DomainVariable_TBL", "Templates");
            //this.Property(t => t.OID).HasColumnName("OID");
            //this.Property(t => t.Name).HasColumnName("Name");
            //this.Property(t => t.Description).HasColumnName("Description");
            //this.Property(t => t.DataType).HasColumnName("DataType");
            //this.Property(t => t.DomainId).HasColumnName("DomainId");
            this.Property(t => t.VariableTypeId).HasColumnName("VariableTypeId");
            this.Property(t => t.RoleId).HasColumnName("RoleTermId");
            //this.Property(t => t.Label).HasColumnName("Label");
            this.Property(t => t.UsageId).HasColumnName("UsageTermId");
            this.Property(t => t.controlledTerminologyId).HasColumnName("DictionaryId");

            // Relationships
            this.HasRequired(t => t.Domain)
                .WithMany(t => t.Variables)
                .HasForeignKey(d => d.DomainId);

            this.HasOptional(t => t.Role)
                .WithMany()
                .HasForeignKey(t => t.RoleId);

            this.HasOptional(t => t.Usage)
                .WithMany()
                .HasForeignKey(t => t.UsageId);

            this.HasOptional(t => t.VariableType)
                .WithMany()
                .HasForeignKey(t => t.VariableTypeId);

            this.HasOptional(t => t.controlledTerminology)
                .WithMany()
                .HasForeignKey(t => t.controlledTerminologyId);

        }
    }
}
