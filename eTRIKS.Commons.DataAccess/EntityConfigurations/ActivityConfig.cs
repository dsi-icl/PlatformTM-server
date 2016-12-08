using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class ActivityConfig : EntityTypeConfiguration<Activity>
    {
        public override void Configure(EntityTypeBuilder<Activity> builder)
        {
           
            // Primary Key
            builder
                .HasKey(t => t.Id);

            // Table & Column Mappings
            builder
               .ToTable("Activity_TBL");
            builder
                .Property(t => t.Id)
                .HasColumnName("ActivityId");
            builder
                .Property(t => t.ProjectId)
                .HasColumnName("ProjectId");
            builder
               .Property(t => t.Name)
               .HasMaxLength(2000);

            // Relationships
            builder.HasOne(t => t.Project)
                .WithMany(s => s.Activities)
                .IsRequired()
                .HasForeignKey(t => t.ProjectId);

            builder
             .HasDiscriminator<string>("Discriminator")
            .HasValue<Activity>("Activity")
            .HasValue<Assay>("Assay");
        }
    }
}
