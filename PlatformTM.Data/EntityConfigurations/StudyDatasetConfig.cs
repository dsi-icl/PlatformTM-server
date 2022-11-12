using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.JoinEntities;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{
    public class StudyDatasetConfig : EntityTypeConfiguration<StudyDataset>
    {
        public override void Configure(EntityTypeBuilder<StudyDataset> builder)
        {
            //builder
            //    .HasKey(t => new { t.DatasetId, t.StudyId });

            //builder
            //   .ToTable("Study_Datasets");

            //builder
            //    .HasOne(dd => dd.Dataset)
            //    .WithMany(d => d.Studies)
            //    .HasForeignKey(dd => dd.DatasetId);

            //builder
            //    .HasOne(dd => dd.Study)
            //    .WithMany(d => d.Datasets)
            //    .HasForeignKey(dd => dd.StudyId);
                
        }
    }
}
