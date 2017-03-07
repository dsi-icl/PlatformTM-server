﻿using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
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
                .WithOne()
                .HasConstraintName("FK_Characteristic_Dataset")
                .HasForeignKey<Characteristic>(k => k.DatasetId);

            builder.HasOne(p => p.Datafile)
                .WithOne()
                .HasConstraintName("FK_Characteristic_DataFile")
                .HasForeignKey<Characteristic>(k => k.DatafileId);

            builder
            .HasDiscriminator<string>("Discriminator")
            .HasValue<SubjectCharacteristic>("SubjectCharacteristic")
            .HasValue<SampleCharacteristic>("SampleCharacteristic");
        }
    }
}