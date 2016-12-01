using eTRIKS.Commons.Core.JoinEntities;
using eTRIKS.Commons.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
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
