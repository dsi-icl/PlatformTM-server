using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.BMO;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{
    public class StudyConfig : EntityTypeConfiguration<Study>
    {
        public override void Configure(EntityTypeBuilder<Study> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Id)
                .IsRequired();
            //.HasMaxLength(200);

            builder.Property(t => t.Name)
                .HasMaxLength(20);

            builder.Property(t => t.Description)
                .HasMaxLength(40);

            // Table & Column Mappings
            builder.ToTable("Studies");
            builder.Property(t => t.Id).HasColumnName("StudyId");

            // Relationships
            builder.HasOne(t => t.Project)
                .WithMany(s => s.Studies)
                .IsRequired()
                .HasForeignKey(t => t.ProjectId);
        }
    }
}
