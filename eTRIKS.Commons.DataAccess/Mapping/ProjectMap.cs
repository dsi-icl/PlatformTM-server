using System;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class ProjectMap : EntityTypeConfiguration<Project>
    {
        //public ProjectMap()
        //{
        //    // Primary Key
        //    this.HasKey(t => t.Id);

            
        //    // Table & Column Mappings
        //    this.ToTable("Project_TBL");
        //    this.Property(t => t.Id).HasColumnName("ProjectId");

        //    this.HasMany(t => t.Users)
        //        .WithMany(t => t.AffiliatedProjects)
        //        .Map(mc =>
        //        {
        //            mc.ToTable("Project_Users");
        //            mc.MapLeftKey("ProjectId");
        //            mc.MapRightKey("UserId");

        //        });

        //    this.HasRequired(t => t.Owner)
        //        .WithMany()
        //        .HasForeignKey(t => t.OwnerId);


        //}

        public override void Map(EntityTypeBuilder<Project> builder)
        {
            throw new NotImplementedException();
        }
    }
}
