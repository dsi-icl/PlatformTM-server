using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.Domain.Model.ControlledTerminology;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{
    public class DBxrefConfig : EntityTypeConfiguration<Dbxref>
    {
        public override void Configure(EntityTypeBuilder<Dbxref> builder)
        {
            // Primary Key
            builder.HasKey(t => t.OID);

            // Properties
            builder.Property(t => t.OID)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(t => t.Accession)
                .HasMaxLength(20);

            builder.Property(t => t.Description)
                .HasMaxLength(127);

            builder.Property(t => t.DBId)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            builder.ToTable("DBxreferences");
            builder.Property(t => t.OID).HasColumnName("OID");

            // Relationships
            builder
                .HasOne(t => t.DB)
                .WithMany()
                .IsRequired()
                .HasForeignKey(t => t.DBId);
        }
    }
}
