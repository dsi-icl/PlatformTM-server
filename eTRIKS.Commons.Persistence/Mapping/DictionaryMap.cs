using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace eTRIKS.Commons.Persistence.Models.Mapping
{
    public class DictionaryMap : EntityTypeConfiguration<Dictionary>
    {
        public DictionaryMap()
        {
            // Primary Key
            this.HasKey(t => t.OID);

            // Properties
            this.Property(t => t.OID)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Name)
                .HasMaxLength(2000);

            this.Property(t => t.Version)
                .HasMaxLength(200);

            this.Property(t => t.URI)
                .HasMaxLength(2000);

            // Table & Column Mappings
            this.ToTable("Dictionary_TAB", "eTRIKSdata");
            this.Property(t => t.OID).HasColumnName("OID");
            this.Property(t => t.Name).HasColumnName("name");
            this.Property(t => t.Version).HasColumnName("version");
            this.Property(t => t.URI).HasColumnName("url");
        }
    }
}
