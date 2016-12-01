using eTRIKS.Commons.Core.Domain.Model.DesignElements;
using eTRIKS.Commons.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class VisitConfig : EntityTypeConfiguration<Visit>
    {
        public override void Configure(EntityTypeBuilder<Visit> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .HasMaxLength(2000);

            builder.Property(t => t.StudyId)
                .IsRequired();
            //.HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("Visit_TBL");
            builder.Property(t => t.Id).HasColumnName("VisitId");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.StudyId).HasColumnName("StudyId");

            // Relationships
            builder.HasOne(t => t.Study)
                .WithMany(s => s.Visits)
                .IsRequired()
                .HasForeignKey(t => t.StudyId);

            //this.HasMany(t => t.TimePoints)
            //    .WithMany()
            //    .Map(mc =>
            //    {
            //        mc.ToTable("Visit_TimePoints");
            //        mc.MapLeftKey("VisitId");
            //        mc.MapRightKey("TimePointId");

            //    });

            builder
                .HasOne(t => t.StudyDay)
                .WithOne();


            //.WithOptionalDependent().Map(m => m.MapKey("StudyDayId"));
        }
    }
}
