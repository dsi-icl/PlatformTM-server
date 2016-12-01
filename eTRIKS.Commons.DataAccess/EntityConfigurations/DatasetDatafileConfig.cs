using eTRIKS.Commons.Core.JoinEntities;
using eTRIKS.Commons.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class DatasetDatafileConfig : EntityTypeConfiguration<DatasetDatafile>
    {
        public override void Configure(EntityTypeBuilder<DatasetDatafile> builder)
        {
            builder
                .HasKey(t => new { t.DatasetId, t.DatafileId });

            builder
               .ToTable("Dataset_DataFiles");

            builder
                .HasOne(dd => dd.Datafile)
                .WithMany(d => d.Datasets)
                .HasForeignKey(dd => dd.DatafileId);

            builder
                .HasOne(dd => dd.Dataset)
                .WithMany(d => d.DataFiles)
                .HasForeignKey(dd => dd.DatasetId);
                
        }
    }
}
