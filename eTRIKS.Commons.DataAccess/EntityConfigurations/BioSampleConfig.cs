using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class BioSampleConfig : EntityTypeConfiguration<Biosample>
    {
        public override void Configure(EntityTypeBuilder<Biosample> builder)
        {
            builder
                .HasKey(t => t.Id);

            builder.Property(t => t.Id)
               .IsRequired();
            builder
                .ToTable("BioSamples");
            builder
                .Property(t => t.Id)
                .HasColumnName("BioSampleDBId");

            builder
                .HasOne(b => b.CollectionStudyDay)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasOne(b => b.CollectionStudyTimePoint)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasOne(b => b.Visit)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
