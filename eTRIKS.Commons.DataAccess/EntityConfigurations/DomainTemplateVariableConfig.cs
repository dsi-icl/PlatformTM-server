using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class DomainTemplateVariableConfig : EntityTypeConfiguration<DatasetTemplateField>
    {
        public override void Configure(EntityTypeBuilder<DatasetTemplateField> builder)
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

            builder.Property(t => t.TemplateId)
                .IsRequired()
                .HasMaxLength(200);

            //builder.Property(t => t.VariableTypeId)
            //    .HasMaxLength(200);

            builder.Property(t => t.RoleId)
                .HasMaxLength(200);

            builder.Property(t => t.UsageId)
                .HasMaxLength(200);

            builder.Property(t => t.ControlledVocabularyId)
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("DomainTemplateVariables");
            builder.Property(t => t.Id).HasColumnName("OID");

            builder.Property(t => t.RoleId).HasColumnName("RoleTermId");
            builder.Property(t => t.UsageId).HasColumnName("UsageTermId");
            builder.Property(t => t.ControlledVocabularyId).HasColumnName("CVTermsDictionaryId");

            // Relationships
            builder.HasOne(t => t.Template)
                .WithMany(t => t.Fields)
                .IsRequired()
                .HasForeignKey(d => d.TemplateId);

            builder.HasOne(t => t.Role)
                .WithMany()
                .HasForeignKey(t => t.RoleId);

            builder.HasOne(t => t.Usage)
                .WithMany()
                .HasForeignKey(t => t.UsageId);

            //builder.HasOne(t => t.VariableType)
            //    .WithMany()
            //    .HasForeignKey(t => t.VariableTypeId);

            builder.HasOne(t => t.ControlledVocabulary)
                .WithMany()
                .HasForeignKey(t => t.ControlledVocabularyId);
        }
    }
}
