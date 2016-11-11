using eTRIKS.Commons.DataAccess.Helpers;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Microsoft.EntityFrameworkCore;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class ArmMap : EntityTypeConfiguration<Arm>
    {
        //public ArmMap()
        //{
        //    // Primary Key
        //    this.HasKey(t => t.Id);

        //    this.Property(t => t.Name)
        //        .IsRequired()
        //        .HasMaxLength(2000);

        //    this.Property(t => t.Code)
        //        .IsRequired();
        //        //.HasMaxLength(200);

        //    // Table & Column Mappings
        //    this.ToTable("Arm_TBL");
        //    this.Property(t => t.Id).HasColumnName("ArmId");
        //    this.Property(t => t.Name).HasColumnName("ArmName");
        //    this.Property(t => t.Code).HasColumnName("ArmCode");

        //    // Relationships
        //    //this.HasRequired(t => t.Project)
        //    //    .WithMany(s => s.Activities)
        //    //    .HasForeignKey(t => t.ProjectId);

        //    this.HasMany(t => t.Studies)
        //        .WithMany(t => t.Arms)
        //        .Map(mc =>
        //        {
        //            mc.ToTable("Study_Arms");
        //            mc.MapLeftKey("ArmId");
        //            mc.MapRightKey("StudyId");

        //        });

        //}

        public override void Map(EntityTypeBuilder<Arm> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(t => t.Code)
                .IsRequired();
            //.HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("Arm_TBL");
            builder.Property(t => t.Id).HasColumnName("ArmId");
            builder.Property(t => t.Name).HasColumnName("ArmName");
            builder.Property(t => t.Code).HasColumnName("ArmCode");

            // Relationships
            //this.HasRequired(t => t.Project)
            //    .WithMany(s => s.Activities)
            //    .HasForeignKey(t => t.ProjectId);

           
         

            //builder.HasMany(t => t.Studies)
            //    .WithMany(t => t.Arms)
            //    .Map(mc =>
            //    {
            //        mc.ToTable("Study_Arms");
            //        mc.MapLeftKey("ArmId");
            //        mc.MapRightKey("StudyId");

            //    });
        }
    }
}
