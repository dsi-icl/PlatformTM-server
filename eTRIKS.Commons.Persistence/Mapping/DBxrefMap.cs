using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class DBxrefMap : EntityTypeConfiguration<Dbxref>
    {
        public DBxrefMap()
        {
            // Primary Key
            this.HasKey(t => t.OID);

            // Properties
            this.Property(t => t.OID)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Accession)
                .HasMaxLength(200);

            this.Property(t => t.Description)
                .HasMaxLength(2000);

            this.Property(t => t.DBId)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("DBxref_TBL");
            //this.Property(t => t.OID).HasColumnName("OID");
            //this.Property(t => t.Accession).HasColumnName("accession");
            //this.Property(t => t.Description).HasColumnName("description");
            //this.Property(t => t.DBId).HasColumnName("db_id");

            // Relationships
            this.HasRequired(t => t.DB).WithMany().HasForeignKey(t => t.DBId);

        }
    }
}
