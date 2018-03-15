using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.JoinEntities;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{
    public class ProjectUserConfig : EntityTypeConfiguration<ProjectUser>
    {
        public override void Configure(EntityTypeBuilder<ProjectUser> builder)
        {
            builder
                .HasKey(t => new { t.ProjectId, t.UserId });

            builder
               .ToTable("Project_Users");

            builder
                .HasOne(dd => dd.User)
                .WithMany(d => d.AffiliatedProjects)
                .HasForeignKey(dd => dd.UserId);

            builder
                .HasOne(dd => dd.Project)
                .WithMany(d => d.Users)
                .HasForeignKey(dd => dd.ProjectId);
                
        }
    }
}
