using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.Domain.Model.Templates;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{
    public class DomainTemplateConfig : EntityTypeConfiguration<DatasetTemplate>
    {
        public override void Configure(EntityTypeBuilder<DatasetTemplate> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Id)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(t => t.Domain)
                .HasMaxLength(127);

            builder.Property(t => t.Class)
                .HasMaxLength(127);

            builder.Property(t => t.Description)
                .HasMaxLength(127);

            builder.Property(t => t.Structure)
                .HasMaxLength(127);

            builder.Property(t => t.Code)
                .HasMaxLength(127);

            // Table & Column Mappings
            builder.ToTable("DomainTemplates");
            builder.Property(t => t.Id).HasColumnName("OID");
        }
    }
}
