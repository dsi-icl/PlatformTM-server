using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace eTRIKS.Commons.Persistence.Models.Mapping
{
    public class DBxrefMap : EntityTypeConfiguration<DBxref>
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
            this.ToTable("DBxref_TAB", "eTRIKSdata");
            this.Property(t => t.OID).HasColumnName("OID");
            this.Property(t => t.Accession).HasColumnName("accession");
            this.Property(t => t.Description).HasColumnName("description");
            this.Property(t => t.DBId).HasColumnName("db");

            // Relationships
            this.HasRequired(t => t.DB);

        }
    }
}
