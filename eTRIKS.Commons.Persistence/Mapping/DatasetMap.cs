using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class DatasetMap : EntityTypeConfiguration<Dataset>
    {
        public DatasetMap()
        {
            // Primary Key
            this.HasKey(t => t.OID);

            // Properties
            this.Property(t => t.DomainId)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.DataFile)
                .HasMaxLength(2000);

            this.Property(t => t.ActivityId)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.OID)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Dataset_TAB", "eTRIKSdata");
            this.Property(t => t.DomainId).HasColumnName("domainId");
            this.Property(t => t.DataFile).HasColumnName("dataFile");
            this.Property(t => t.ActivityId).HasColumnName("activityId");
            this.Property(t => t.OID).HasColumnName("OID");

            // Relationships
            this.HasRequired(d => d.Activity)
                .WithMany(a => a.Datasets)
                .HasForeignKey(d => d.ActivityId);

            this.HasRequired(d => d.Domain);



        }
    }
}
