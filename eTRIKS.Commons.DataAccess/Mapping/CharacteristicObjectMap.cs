using System;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class CharacteristicObjectMap : EntityTypeConfiguration<CharacteristicObject>
    {
        //public CharacteristicObjectMap()
        //{
        //    // Primary Key
        //    this.HasKey(t => t.Id);

        //    // Properties
           
            

        //    // Table & Column Mappings
        //    this.ToTable("CharacteristicObjects");
        //    this.Property(t => t.Id).HasColumnName("CharacteristicObjId");
        //    //this.Property(t => t.Code).HasColumnName("Code");
        //    //this.Property(t => t.Name).HasColumnName("Name");
        //    //this.Property(t => t.Definition).HasColumnName("Definition");
        //    //this.Property(t => t.Order).HasColumnName("order");
        //    //this.Property(t => t.Rank).HasColumnName("rank");
        //    //this.Property(t => t.IsUserSpecified).HasColumnName("userSpecified");
        //    //this.Property(t => t.DictionartyId).HasColumnName("dictionaryId");
        //    //this.Property(t => t.XrefId).HasColumnName("xref_id");

        //    // Relationships
        //    this.HasOptional(t => t.ControlledTerm)
        //        .WithMany()
        //        .HasForeignKey(t => t.CVtermId);
               
        //    this.HasRequired(t => t.Project)
        //        .WithMany()
        //        .HasForeignKey(d => d.ProjectId);

        //}

        public override void Map(EntityTypeBuilder<CharacteristicObject> builder)
        {
            throw new NotImplementedException();
        }
    }
}
