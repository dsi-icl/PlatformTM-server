using System;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class SubjectMap : EntityTypeConfiguration<HumanSubject>
    {
        //public SubjectMap()
        //{
        //    // Primary Key
        //    this.HasKey(t => t.Id);

        //    this.Property(t => t.Id)
        //       .IsRequired();
        //       //.HasMaxLength(200);

        //    //this.Property(t => t.DataFile)
        //    //    .HasMaxLength(2000);

        //    //this.Property(t => t.ActivityId)
        //    //    .IsRequired()
        //    //    .HasMaxLength(200);

           
        //    // Table & Column Mappings
        //    this.ToTable("Subject_TBL");
        //    this.Property(t => t.Id).HasColumnName("SubjectDBId");
        //    //this.Property(t => t.DomainId).HasColumnName("domainId");
        //    //this.Property(t => t.DataFile).HasColumnName("DataFile");
        //    //this.Property(t => t.ActivityId).HasColumnName("ActivityId");

        //    // Relationships
        //    //this.HasRequired(d => d.Activity)
        //    //    .WithMany(a => a.Datasets)
        //    //    .HasForeignKey(d => d.ActivityId);

        //    //this.HasRequired(d => d.Domain)
        //    //    .WithMany()
        //    //    .HasForeignKey(t => t.DomainId);



        //}

        public override void Map(EntityTypeBuilder<HumanSubject> builder)
        {
            throw new NotImplementedException();
        }
    }
}
