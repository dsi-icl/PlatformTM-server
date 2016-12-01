using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class CharacteristicObjectConfig : EntityTypeConfiguration<CharacteristicObject>
    {
        public override void Configure(EntityTypeBuilder<CharacteristicObject> builder)
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

            builder.HasOne(t => t.Project)
                .WithMany()
                .IsRequired()
                .HasForeignKey(d => d.ProjectId);
        }
    }
}
