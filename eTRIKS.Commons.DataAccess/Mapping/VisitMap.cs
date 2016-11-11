using System;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class VisitMap : EntityTypeConfiguration<Visit>
    {
        //public VisitMap()
        //{
        //    // Primary Key
        //    this.HasKey(t => t.Id);

        //    this.Property(t => t.Name)
        //        .HasMaxLength(2000);

        //    this.Property(t => t.StudyId)
        //        .IsRequired();
        //        //.HasMaxLength(200);

        //    // Table & Column Mappings
        //    this.ToTable("Visit_TBL");
        //    this.Property(t => t.Id).HasColumnName("VisitId");
        //    this.Property(t => t.Name).HasColumnName("Name");
        //    this.Property(t => t.StudyId).HasColumnName("StudyId");

        //    // Relationships
        //    this.HasRequired(t => t.Study)
        //        .WithMany(s => s.Visits)
        //        .HasForeignKey(t => t.StudyId);

            
        //    this.HasMany(t => t.TimePoints)
        //        .WithMany()
        //        .Map(mc =>
        //        {
        //            mc.ToTable("Visit_TimePoints");
        //            mc.MapLeftKey("VisitId");
        //            mc.MapRightKey("TimePointId");

        //        });

        //   // this.HasOptional(t => t.StudyDay).WithOptionalDependent().Map(m => m.MapKey("StudyDayId"));


        //    //this.HasMany(t => t.Studies)
        //    //    .WithMany(t => t.Activities)
        //    //    .Map(mc =>
        //    //    {
        //    //        mc.ToTable("Study_Activities");
        //    //        mc.MapLeftKey("StudyId");
        //    //        mc.MapRightKey("ActivityId");

        //    //    });

        //}

        public override void Map(EntityTypeBuilder<Visit> builder)
        {
            throw new NotImplementedException();
        }
    }
}
