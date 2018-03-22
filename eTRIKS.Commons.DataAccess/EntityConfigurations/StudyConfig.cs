using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
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
                .HasMaxLength(2000);

            builder.Property(t => t.Description)
                .HasMaxLength(2000);

            // Table & Column Mappings
            builder.ToTable("Studies");
            builder.Property(t => t.Id).HasColumnName("StudyId");

            // Relationships
            builder.HasOne(t => t.Project)
                .WithMany(s => s.Studies)
                .IsRequired()
                .HasForeignKey(t => t.ProjectId);


            //this.HasMany(t => t.Observations)
            //    .WithMany(t => t.Studies)
            //    .Map(mc =>
            //    {
            //        mc.ToTable("Study_Observations");
            //        mc.MapLeftKey("StudyId");
            //        mc.MapRightKey("ObservationId");

            //    });
            //this.HasMany(t => t.Datasets)
            //    .WithMany(t => t.Studies)
            //    .Map(mc =>
            //    {
            //        mc.ToTable("Study_Datasets");
            //        mc.MapLeftKey("StudyId");
            //        mc.MapRightKey("DatasetId");

            //    });
        }
    }
}
