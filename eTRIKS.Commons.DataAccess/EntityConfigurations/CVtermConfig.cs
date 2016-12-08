using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class CVtermConfig : EntityTypeConfiguration<CVterm>
    {
        public override void Configure(EntityTypeBuilder<CVterm> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Id)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Code)
                .HasMaxLength(200);

            builder.Property(t => t.Name)
                .HasMaxLength(2000);

            builder.Property(t => t.Definition)
                .HasMaxLength(2000);

            builder.Property(t => t.Synonyms)
                .HasMaxLength(2000);

            builder.Property(t => t.IsUserSpecified);

            builder.Property(t => t.DictionaryId)
                .HasMaxLength(200);

            builder.Property(t => t.XrefId)
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("CVterm_TBL");
            builder.Property(t => t.Id).HasColumnName("OID");

            // Relationships
            builder.HasOne(t => t.Xref)
                .WithMany()
                .HasForeignKey(t => t.XrefId);

            builder.HasOne(t => t.Dictionary)
                .WithMany(d => d.Terms)
                .IsRequired()
                .HasForeignKey(d => d.DictionaryId);
        }
    }
}
