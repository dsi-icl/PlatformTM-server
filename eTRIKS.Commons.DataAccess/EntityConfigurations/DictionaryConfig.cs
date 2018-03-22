using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class DictionaryConfig : EntityTypeConfiguration<Dictionary>
    {
        public override void Configure(EntityTypeBuilder<Dictionary> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Id)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(t => t.Name)
                .HasMaxLength(2000);
            builder.Property(t => t.Definition)
               .HasMaxLength(2000);
            builder.Property(t => t.XrefId)
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("Dictionaries");
            builder.Property(t => t.Id).HasColumnName("OID");

            // Relationships
            builder
                .HasOne(t => t.Xref)
                .WithMany()
                .HasForeignKey(t => t.XrefId);

            builder.HasMany(t => t.Terms)
                .WithOne(t => t.Dictionary)
                .IsRequired()
                .HasForeignKey(t => t.DictionaryId);
        }
    }
}
