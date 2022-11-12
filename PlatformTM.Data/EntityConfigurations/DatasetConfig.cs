using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{
    public class DatasetConfig : EntityTypeConfiguration<Dataset>
    {
        public override void Configure(EntityTypeBuilder<Dataset> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
               .IsRequired();
            //.HasMaxLength(200);

            // Properties
            builder.Property(t => t.TemplateId)
                .IsRequired()
                .HasMaxLength(10);
            // Table & Column Mappings
            builder.ToTable("Datasets");
            builder.Property(t => t.Id).HasColumnName("OID");

            // Relationships
            builder.HasOne(d => d.Activity)
                .WithMany(a => a.Datasets)
                .IsRequired()
                .HasForeignKey(d => d.ActivityId);

            builder.HasOne(d => d.Template)
                .WithMany()
                .IsRequired()
                .HasForeignKey(t => t.TemplateId);
        }
    }
}
