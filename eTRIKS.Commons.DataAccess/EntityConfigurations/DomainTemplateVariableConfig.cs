using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class DomainTemplateVariableConfig : EntityTypeConfiguration<DomainVariableTemplate>
    {
        public override void Configure(EntityTypeBuilder<DomainVariableTemplate> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Id)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Name)
                .HasMaxLength(2000);

            builder.Property(t => t.Description)
                .HasMaxLength(2000);

            builder.Property(t => t.DataType)
                .HasMaxLength(200);

            builder.Property(t => t.Label)
                .HasMaxLength(2000);

            builder.Property(t => t.DomainId)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.VariableTypeId)
                .HasMaxLength(200);

            builder.Property(t => t.RoleId)
                .HasMaxLength(200);

            builder.Property(t => t.UsageId)
                .HasMaxLength(200);

            builder.Property(t => t.controlledTerminologyId)
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("Templates.DomainVariable_TBL");
            builder.Property(t => t.Id).HasColumnName("OID");

            builder.Property(t => t.VariableTypeId).HasColumnName("VariableTypeId");
            builder.Property(t => t.RoleId).HasColumnName("RoleTermId");
            //this.Property(t => t.Label).HasColumnName("Label");
            builder.Property(t => t.UsageId).HasColumnName("UsageTermId");
            builder.Property(t => t.controlledTerminologyId).HasColumnName("DictionaryId");

            // Relationships
            builder.HasOne(t => t.Domain)
                .WithMany(t => t.Variables)
                .IsRequired()
                .HasForeignKey(d => d.DomainId);

            builder.HasOne(t => t.Role)
                .WithMany()
                .HasForeignKey(t => t.RoleId);

            builder.HasOne(t => t.Usage)
                .WithMany()
                .HasForeignKey(t => t.UsageId);

            builder.HasOne(t => t.VariableType)
                .WithMany()
                .HasForeignKey(t => t.VariableTypeId);

            builder.HasOne(t => t.controlledTerminology)
                .WithMany()
                .HasForeignKey(t => t.controlledTerminologyId);
        }
    }
}
