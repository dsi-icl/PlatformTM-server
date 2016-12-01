using eTRIKS.Commons.Core.JoinEntities;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class StudyArmConfig : EntityTypeConfiguration<StudyArm>
    {
        public override void Configure(EntityTypeBuilder<StudyArm> builder)
        {
            builder
                .HasKey(t => new { t.ArmId, t.StudyId });

            builder
               .ToTable("Study_Arms");

            builder
                .HasOne(dd => dd.Arm)
                .WithMany(d => d.Studies)
                .HasForeignKey(dd => dd.ArmId);

            builder
                .HasOne(dd => dd.Study)
                .WithMany(d => d.Arms)
                .HasForeignKey(dd => dd.StudyId);
                
        }
    }
}
