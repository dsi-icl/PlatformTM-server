using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.JoinEntities;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{
    public class DatasetDatafileConfig : EntityTypeConfiguration<DatasetDatafile>
    {
        public override void Configure(EntityTypeBuilder<DatasetDatafile> builder)
        {
            //builder
            //    .HasKey(t => new { t.DatasetId, t.DatafileId });

            //builder
            //   .ToTable("Dataset_DataFiles");

            //builder
            //    .HasOne(dd => dd.Datafile)
            //    .WithMany(d => d.Datasets)
            //    .HasForeignKey(dd => dd.DatafileId);

            //builder
            //    .HasOne(dd => dd.Dataset)
            //    .WithMany(d => d.DataFiles)
            //    .HasForeignKey(dd => dd.DatasetId);
                
        }
    }
}
