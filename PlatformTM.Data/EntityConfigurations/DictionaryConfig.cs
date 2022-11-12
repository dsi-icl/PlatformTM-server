using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.Domain.Model.ControlledTerminology;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
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
                .HasMaxLength(10);
            builder.Property(t => t.Name)
                .HasMaxLength(20);
            builder.Property(t => t.Definition)
               .HasMaxLength(40);
            builder.Property(t => t.XrefId)
                .HasMaxLength(10);

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
