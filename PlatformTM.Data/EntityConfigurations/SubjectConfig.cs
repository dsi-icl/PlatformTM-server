using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{
    public class SubjectConfig : EntityTypeConfiguration<HumanSubject>
    {
        public override void Configure(EntityTypeBuilder<HumanSubject> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);
            builder.ToTable("Subjects");
            builder.Property(t => t.Id).HasColumnName("SubjectDBId");

            // Relationships
            builder.HasOne(t => t.StudyCohort)
                   .WithMany()
                   .HasForeignKey(t => t.StudyCohortId);

            builder.HasOne(t => t.Study)
               .WithMany(s=>s.Subjects)
               .IsRequired()
               .HasForeignKey(t => t.StudyId);

            builder.HasOne(t => t.Dataset)
                   .WithMany()
                   .IsRequired()
                   .HasForeignKey(t => t.DatasetId);

            builder.HasOne(t => t.SourceDataFile)
                   .WithMany()
                   .IsRequired()
                   .HasForeignKey(t => t.SourceDatafileId);
        }
    }
}
