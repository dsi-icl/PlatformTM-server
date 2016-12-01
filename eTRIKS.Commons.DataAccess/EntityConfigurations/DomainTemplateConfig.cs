using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class DomainTemplateConfig : EntityTypeConfiguration<DomainTemplate>
    {
        public override void Configure(EntityTypeBuilder<DomainTemplate> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Id)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Name)
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
            builder.ToTable("Templates.DomainDataset_TBL");
            builder.Property(t => t.Id).HasColumnName("OID");
        }
    }
}
