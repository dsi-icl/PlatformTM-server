using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class CVtermMap : EntityTypeConfiguration<CVterm>
    {
        public CVtermMap()
        {
            // Primary Key
            this.HasKey(t => t.OID);

            // Properties
            this.Property(t => t.OID)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Code)
                .HasMaxLength(200);

            this.Property(t => t.Name)
                .HasMaxLength(2000);

            this.Property(t => t.Definition)
                .HasMaxLength(2000);

            this.Property(t => t.Synonyms)
                .HasMaxLength(2000);

            this.Property(t => t.IsUserSpecified);

            this.Property(t => t.DictionaryId)
                .HasMaxLength(200);

            this.Property(t => t.XrefId)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("CVterm_TBL");
            //this.Property(t => t.OID).HasColumnName("OID");
            //this.Property(t => t.Code).HasColumnName("Code");
            //this.Property(t => t.Name).HasColumnName("Name");
            //this.Property(t => t.Definition).HasColumnName("Definition");
            //this.Property(t => t.Order).HasColumnName("order");
            //this.Property(t => t.Rank).HasColumnName("rank");
            //this.Property(t => t.IsUserSpecified).HasColumnName("userSpecified");
            //this.Property(t => t.DictionartyId).HasColumnName("dictionaryId");
            //this.Property(t => t.XrefId).HasColumnName("xref_id");

            // Relationships
            this.HasOptional(t => t.Xref)
                .WithMany()
                .HasForeignKey(t => t.XrefId);
               
            this.HasRequired(t => t.Dictionary)
                .WithMany(d => d.Terms)
                .HasForeignKey(d => d.DictionaryId);

        }
    }
}
