using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class CVtermMap : EntityTypeConfiguration<CVterm>
    {
        public CVtermMap()
        {
            // Primary Key
            this.HasKey(t => t.Code);

            // Properties
            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Name)
                .HasMaxLength(2000);

            this.Property(t => t.DictionartyId)
                .HasMaxLength(200);

            this.Property(t => t.externalReferencId)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("CVterm_TAB", "eTRIKSdata");
            this.Property(t => t.Code).HasColumnName("abbrv");
            this.Property(t => t.Name).HasColumnName("name");
            this.Property(t => t.Order).HasColumnName("order");
            this.Property(t => t.Rank).HasColumnName("rank");
            this.Property(t => t.UserSpecified).HasColumnName("userSpecified");
            this.Property(t => t.DictionartyId).HasColumnName("dictionartyId");
            this.Property(t => t.externalReferencId).HasColumnName("dbref");

            // Relationships
            this.HasOptional(t => t.externalReference);
               
            this.HasRequired(t => t.Dictionary)
                .WithMany(d => d.ControlledTerms)
                .HasForeignKey(d => d.DictionartyId);

        }
    }
}
