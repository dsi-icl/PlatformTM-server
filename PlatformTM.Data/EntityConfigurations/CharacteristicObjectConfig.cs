using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{
    public class CharacteristicObjectConfig : EntityTypeConfiguration<CharacteristicFeature>
    {
        public override void Configure(EntityTypeBuilder<CharacteristicFeature> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);
            // Table & Column Mappings
            builder.ToTable("CharacteristicObjects");
            builder.Property(t => t.Id).HasColumnName("CharacteristicObjId");
            // Relationships
            builder.HasOne(t => t.ControlledTerm)
                .WithMany()
                .HasForeignKey(t => t.CVtermId);

            //builder.HasOne(t => t.Activity)
            //    .WithMany()
            //    .HasForeignKey(t => t.ActivityId);

            builder.HasOne(t => t.Project)
                .WithMany()
                .IsRequired()
                .HasForeignKey(d => d.ProjectId);
        }
    }
}
