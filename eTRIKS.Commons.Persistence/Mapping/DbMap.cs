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

            this.Property(t => t.Name)
                .HasMaxLength(200);

            this.Property(t => t.UrlPrefix)
                .HasMaxLength(2000);

            this.Property(t => t.Url)
                .HasMaxLength(2000);

            this.Property(t => t.Version)
               .HasMaxLength(2000);

            // Table & Column Mappings
            this.ToTable("Db_TBL");
            //this.Property(t => t.OID).HasColumnName("OID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.UrlPrefix).HasColumnName("URLPrefix");
            this.Property(t => t.Url).HasColumnName("URL");
            this.Property(t => t.Version).HasColumnName("Version");
        }
    }
}
