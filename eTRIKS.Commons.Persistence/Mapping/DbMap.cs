using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class DbMap : EntityTypeConfiguration<DB>
    {
        public DbMap()
        {
            // Primary Key
            this.HasKey(t => t.OID);

            // Properties
            this.Property(t => t.OID)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.name)
                .HasMaxLength(200);

            this.Property(t => t.urlPrefix)
                .HasMaxLength(2000);

            this.Property(t => t.url)
                .HasMaxLength(2000);

            // Table & Column Mappings
            this.ToTable("Db_TAB", "eTRIKSdata");
            this.Property(t => t.OID).HasColumnName("OID");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.urlPrefix).HasColumnName("urlPrefix");
            this.Property(t => t.url).HasColumnName("url");
        }
    }
}
