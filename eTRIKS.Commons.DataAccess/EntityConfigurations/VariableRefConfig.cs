using eTRIKS.Commons.Core.Domain.Model.DatasetModel;
using eTRIKS.Commons.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class VariableRefConfig : EntityTypeConfiguration<VariableReference>
    {

        public override void Configure(EntityTypeBuilder<VariableReference> builder)
        {
            // Primary Key
            builder.HasKey(t => new { t.VariableDefinitionId, t.DatasetId });

            // Properties
            builder.Property(t => t.VariableDefinitionId)
                .IsRequired();

            builder.Property(t => t.DatasetId)
                .IsRequired();

            // Table & Column Mappings
            builder.ToTable("Variable_Reference_TBL");
            builder.Property(t => t.VariableDefinitionId).HasColumnName("VariableId");
            builder.Property(t => t.DatasetId).HasColumnName("ActivityDatasetId");
            

            // Relationships
            builder.HasOne(t => t.Dataset)
                .WithMany(t => t.Variables)
                .IsRequired()
                .HasForeignKey(d => d.DatasetId);

            builder.HasOne(t => t.VariableDefinition)
                .WithMany()
                .IsRequired()
                .HasForeignKey(t => t.VariableDefinitionId);

        }
    }
}
