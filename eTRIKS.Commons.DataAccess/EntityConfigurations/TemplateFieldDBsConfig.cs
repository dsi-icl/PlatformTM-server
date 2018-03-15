using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.JoinEntities;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{
    public class TemplateFieldDBsConfig : EntityTypeConfiguration<TemplateFieldDB>
    {
        public override void Configure(EntityTypeBuilder<TemplateFieldDB> builder)
        {
            builder
                .HasKey(t => new { t.TemplateFieldId, t.TermSourceId });

            builder
               .ToTable("TemplateField_TermSource");

            builder
                .HasOne(dd => dd.TemplateField)
                .WithMany(tt=>tt.FieldTermSources).HasConstraintName("FK_TemplateField_TermSource")
                .HasForeignKey(dd => dd.TemplateFieldId);

            builder
                .HasOne(dd => dd.TermSource)
                .WithMany().HasConstraintName("FK_TermSource_TemplateField")
                .HasForeignKey(dd => dd.TermSourceId);
        }
    }
}
