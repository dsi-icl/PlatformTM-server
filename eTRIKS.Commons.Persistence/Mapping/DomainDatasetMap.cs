using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model.Templates;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class DomainTemplateMap : EntityTypeConfiguration<DomainTemplate>
    {
        public DomainTemplateMap()
        {
            // Primary Key
            this.HasKey(t => t.OID);

            // Properties
            this.Property(t => t.OID)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Name)
                .HasMaxLength(2000);

            this.Property(t => t.Class)
                .HasMaxLength(200);

            this.Property(t => t.Description)
                .HasMaxLength(2000);

            this.Property(t => t.Structure)
                .HasMaxLength(200);

            this.Property(t => t.Code)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Templates.DomainDataset_TBL", "Templates");
            //this.Property(t => t.OID).HasColumnName("OID");
            //this.Property(t => t.Name).HasColumnName("domainName");
            //this.Property(t => t.Class).HasColumnName("class");
            //this.Property(t => t.Description).HasColumnName("description");
            //this.Property(t => t.Structure).HasColumnName("structure");
            //this.Property(t => t.IsRepeating).HasColumnName("repeating");
            //this.Property(t => t.Code).HasColumnName("code");
        }
    }
}
