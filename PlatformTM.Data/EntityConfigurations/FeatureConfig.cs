using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.BMO;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{
    public class FeatureConfig : EntityTypeConfiguration<Feature>
    {
        public override void Configure(EntityTypeBuilder<Feature> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Id)
                .IsRequired();


            builder.Property(t => t.Name)
                .HasMaxLength(100);

            builder.Property(t => t.Category)
                .HasMaxLength(50);
            builder.Property(t => t.Domain)
                .HasMaxLength(40);
            builder.Property(t => t.Subcategory)
                .HasMaxLength(40);
            builder.Property(t => t.ControlledTerm)
               .HasMaxLength(40);
            builder.Property(t => t.TermURI)
               .HasMaxLength(200);


            // Table & Column Mappings
            builder.ToTable("Features");
            builder.Property(t => t.Id).HasColumnName("FeatureId");

            // Relationships
            builder.HasOne(t => t.Dataset)
                .WithMany(s => s.ObservedFeatures)
                .IsRequired()
                .HasForeignKey(t => t.DatasetId);



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
