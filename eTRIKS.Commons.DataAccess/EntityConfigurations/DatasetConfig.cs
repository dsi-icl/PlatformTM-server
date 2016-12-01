using eTRIKS.Commons.Core.Domain.Model.DatasetModel;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
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
            builder.Property(t => t.DomainId)
                .IsRequired()
                .HasMaxLength(200);
            // Table & Column Mappings
            builder.ToTable("Dataset_TBL");
            builder.Property(t => t.Id).HasColumnName("OID");

            // Relationships
            builder.HasOne(d => d.Activity)
                .WithMany(a => a.Datasets)
                .IsRequired()
                .HasForeignKey(d => d.ActivityId);

            builder.HasOne(d => d.Domain)
                .WithMany()
                .IsRequired()
                .HasForeignKey(t => t.DomainId);
        }
    }
}
