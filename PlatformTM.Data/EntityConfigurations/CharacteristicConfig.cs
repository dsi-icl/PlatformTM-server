using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{
    internal class CharacteristicConfig : EntityTypeConfiguration<Characteristic>
    {

        public override void Configure(EntityTypeBuilder<Characteristic> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            builder.ToTable("Characteristics");
            builder.Property(t => t.Id).HasColumnName("CharacteristicId");
            builder.Property(t => t.VerbatimName).HasColumnName("CharacObjName");

            builder.Property(t => t.VerbatimName)
                .HasMaxLength(2000);

            builder.HasOne(p => p.Dataset)
                .WithMany()
                .HasConstraintName("FK_Characteristic_Dataset")
                .HasForeignKey(k => k.DatasetId);

            builder.HasOne(p => p.Datafile)
                   .WithMany()
                   .HasConstraintName("FK_Characteristic_DataFile")
                   .HasForeignKey(k => k.DatafileId);

            builder
            .HasDiscriminator<string>("Discriminator")
            .HasValue<SubjectCharacteristic>("SubjectCharacteristic")
            .HasValue<SampleCharacteristic>("SampleCharacteristic");
        }
    }
}
