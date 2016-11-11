using System;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class ActivityMap : EntityTypeConfiguration<Activity>
    {
        //public ActivityMap()
        //{
        //    // Primary Key
        //    this.HasKey(t => t.Id);

        //    this.Property(t => t.Name)
        //        .HasMaxLength(2000);

        //    this.Property(t => t.ProjectId)
        //        .IsRequired();
        //        //.HasMaxLength(200);

        //    // Table & Column Mappings
        //    this.ToTable("Activity_TBL");
        //    this.Property(t => t.Id).HasColumnName("ActivityId");
        //    this.Property(t => t.Name).HasColumnName("Name");
        //    this.Property(t => t.ProjectId).HasColumnName("ProjectId");

        //    // Relationships
        //    this.HasRequired(t => t.Project)
        //        .WithMany(s => s.Activities)
        //        .HasForeignKey(t => t.ProjectId);

        //    //this.HasMany(t => t.Studies)
        //    //    .WithMany(t => t.Activities)
        //    //    .Map(mc =>
        //    //    {
        //    //        mc.ToTable("Study_Activities");
        //    //        mc.MapLeftKey("StudyId");
        //    //        mc.MapRightKey("ActivityId");

        //    //    });

        //}

        public override void Map(EntityTypeBuilder<Activity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .HasMaxLength(2000);

            builder.Property(t => t.ProjectId)
                .IsRequired();
            //.HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("Activity_TBL");
            builder.Property(t => t.Id).HasColumnName("ActivityId");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ProjectId).HasColumnName("ProjectId");

            // Relationships
            //builder.HasRequired(t => t.Project)
            //    .WithMany(s => s.Activities)
            //    .HasForeignKey(t => t.ProjectId);
        }
    }
}
