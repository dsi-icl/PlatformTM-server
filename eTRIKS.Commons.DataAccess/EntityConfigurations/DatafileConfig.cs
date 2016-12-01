using eTRIKS.Commons.Core.Domain.Model.DatasetModel;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class DatafileConfig : EntityTypeConfiguration<DataFile>
    {
       public override void Configure(EntityTypeBuilder<DataFile> builder)
        {
            // Primary Key
           builder
               .HasKey(t => t.Id);
                

            // Table & Column Mappings
            builder
                .ToTable("DataFiles_TBL");


            builder
                .Property(t => t.Id).HasColumnName("DataFileId");

            // Relationships
            builder.HasOne(t => t.Project)
                .WithMany(s => s.DataFiles)
                .IsRequired()
                .HasForeignKey(t => t.ProjectId);

            /*CONSIDER PUTTING THIS BACK WITH M-2-M RELATIONSHIP IS SUPPORTED*/
            //builder.HasMany(t => t.Datasets)
            //    .WithMany(t => t.DataFiles)
            //    .Map(mc =>
            //    {
            //        mc.ToTable("Dataset_DataFiles");
            //        mc.MapLeftKey("DataFileId");
            //        mc.MapRightKey("DatasetId");

            //    });

        }
    }
}
