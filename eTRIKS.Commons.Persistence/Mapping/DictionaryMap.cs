using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Persistence.Mapping
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
            this.Property(t => t.Definition)
               .HasMaxLength(2000);
            this.Property(t => t.XrefId)
                .HasMaxLength(200)
                .IsOptional();
            
            // Table & Column Mappings
            this.ToTable("Dictionary_TBL");
            //this.Property(t => t.OID).HasColumnName("OID");
            //this.Property(t => t.Name).HasColumnName("name");
            //this.Property(t => t.Definition).HasColumnName("Definition");
            //this.Property(t => t.XrefId).HasColumnName("Xref_id");

            // Relationships
            this.HasOptional(t => t.Xref).WithMany().HasForeignKey(t => t.XrefId);

            this.HasMany(t => t.Terms)
                .WithRequired(t => t.Dictionary)
                .HasForeignKey(t => t.DictionaryId);

        }
    }
}
