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
                .HasMaxLength(200);

            builder.Property(t => t.Domain)
                .HasMaxLength(2000);

            builder.Property(t => t.Class)
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(2000);

            builder.Property(t => t.Structure)
                .HasMaxLength(200);

            builder.Property(t => t.Code)
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("DomainTemplates");
            builder.Property(t => t.Id).HasColumnName("OID");
        }
    }
}
