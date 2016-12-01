using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
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
                .HasMaxLength(200);

            builder.Property(t => t.Accession)
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(2000);

            builder.Property(t => t.DBId)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("DBxref_TBL");
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
