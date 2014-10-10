using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class StudyMap : EntityTypeConfiguration<Study>
    {
        public StudyMap()
        {
            // Primary Key
            this.HasKey(t => t.OID);

            // Properties
            this.Property(t => t.OID)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Name)
                .HasMaxLength(2000);

            this.Property(t => t.Description)
                .HasMaxLength(2000);

            // Table & Column Mappings
            this.ToTable("Study_TBL");
            //this.Property(t => t.OID).HasColumnName("OID");
            //this.Property(t => t.Name).HasColumnName("name");
            //this.Property(t => t.Description).HasColumnName("description");
        }
    }
}
