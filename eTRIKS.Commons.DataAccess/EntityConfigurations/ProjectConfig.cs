using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{
    public class ProjectConfig : EntityTypeConfiguration<Project>
    {
        public override void Configure(EntityTypeBuilder<Project> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);


            // Table & Column Mappings
            builder.ToTable("Projects");
            builder.Property(t => t.Id).HasColumnName("ProjectId");

            //builder.HasMany(t => t.Users)
            //    .WithMany(t => t.AffiliatedProjects)
            //    .Map(mc =>
            //    {
            //        mc.ToTable("Project_Users");
            //        mc.MapLeftKey("ProjectId");
            //        mc.MapRightKey("UserId");

            //    });

            builder.HasOne(t => t.Owner)
                .WithMany()
                .IsRequired()
                .HasForeignKey(t => t.OwnerId);

        }
    }
}
