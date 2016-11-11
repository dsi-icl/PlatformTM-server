using eTRIKS.Commons.DataAccess.Helpers;
using eTRIKS.Commons.Core.Domain.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class AssayMap : EntityTypeConfiguration<Assay>
    {
        //public AssayMap()
        //{
        //    // Primary Key
        //    this.HasKey(t => t.Id);

        //    this.Property(t => t.Id)
        //       .IsRequired();
        //       //.HasMaxLength(200);

        //    // Properties
        //    this.Property(t => t.Id)
        //        .IsRequired();

        //    //this.Property(t => t.DataFile)
        //    //    .HasMaxLength(2000);

        //    //this.Property(t => t.ActivityId)
        //    //    .IsRequired()
        //    //    .HasMaxLength(200);

           
        //    // Table & Column Mappings
        //    this.ToTable("Dataset_TBL");
        //    //this.Property(t => t.DomainId).HasColumnName("domainId");
        //    //this.Property(t => t.DataFile).HasColumnName("DataFile");
        //    //this.Property(t => t.ActivityId).HasColumnName("ActivityId");
        //    this.Property(t => t.Id).HasColumnName("OID");

        //    // Relationships
        //    //this.HasRequired(d => d.Activity)
        //    //    .WithMany(a => a.Datasets)
        //    //    .HasForeignKey(d => d.ActivityId);

        //    //this.HasRequired(d => d.Domain)
        //    //    .WithMany()
        //    //    .HasForeignKey(t => t.DomainId);



        //}

        public override void Map(EntityTypeBuilder<Assay> builder)
        {
            throw new NotImplementedException();
        }
    }
}
