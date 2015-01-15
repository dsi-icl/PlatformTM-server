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

            this.Property(t => t.OID)
               .IsRequired();
               //.HasMaxLength(200);

            // Properties
            this.Property(t => t.DomainId)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.DataFile)
                .HasMaxLength(2000);

            //this.Property(t => t.ActivityId)
            //    .IsRequired()
            //    .HasMaxLength(200);

           
            // Table & Column Mappings
            this.ToTable("Dataset_TBL");
            //this.Property(t => t.DomainId).HasColumnName("domainId");
            //this.Property(t => t.DataFile).HasColumnName("DataFile");
            //this.Property(t => t.ActivityId).HasColumnName("ActivityId");
            //this.Property(t => t.OID).HasColumnName("OID");

            // Relationships
            this.HasRequired(d => d.Activity)
                .WithMany(a => a.Datasets)
                .HasForeignKey(d => d.ActivityId);

            //A hack to get EF to use DomainId as the FK for Domain and not to autogenerate another one
            this.HasRequired(d => d.Domain).WithMany().HasForeignKey(t => t.DomainId);



        }
    }
}
