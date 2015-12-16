using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model;
using System.Data.Entity.ModelConfiguration;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class DataFileMap : EntityTypeConfiguration<DataFile>
    {
        public DataFileMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Table & Column Mappings
            this.ToTable("DataFiles_TBL");
            this.Property(t => t.Id).HasColumnName("DataFileId");

            // Relationships
            this.HasRequired(t => t.Project)
                .WithMany(s => s.DataFiles)
                .HasForeignKey(t => t.ProjectId);


            this.HasMany(t => t.Datasets)
                .WithMany(t => t.DataFiles)
                .Map(mc =>
                {
                    mc.ToTable("Dataset_DataFiles");
                    mc.MapLeftKey("DataFileId");
                    mc.MapRightKey("DatasetId");

                });
        }
    }
}
