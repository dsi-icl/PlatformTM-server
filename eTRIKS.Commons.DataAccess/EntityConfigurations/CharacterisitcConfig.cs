using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    internal class CharacterisitcConfig : EntityTypeConfiguration<Characterisitc>
    {

        public override void Configure(EntityTypeBuilder<Characterisitc> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            builder.ToTable("Characteristics_TBL");
            builder.Property(t => t.Id).HasColumnName("CharacterisitcId");
            builder.Property(t => t.VerbatimName).HasColumnName("CharacObjName");

            builder.Property(t => t.VerbatimName)
                .HasMaxLength(2000);

            builder.HasOne(p => p.DatasetVariable)
                .WithOne()
                .HasForeignKey<Characterisitc>(k => new { k.DatasetVariableId, k.DatasetId });

            builder
             .HasDiscriminator<string>("Discriminator")
            .HasValue<SubjectCharacteristic>("SubjectCharacteristic")
            .HasValue<SampleCharacteristic>("SampleCharacteristic");
        }
    }
}
