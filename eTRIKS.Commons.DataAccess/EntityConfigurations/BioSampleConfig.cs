using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore;
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
                .ToTable("BioSamples_TBL");
            builder
                .Property(t => t.Id)
                .HasColumnName("BioSampleDBId");
        }
    }
}
