using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.JoinEntities;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{
    public class VariableQualifierConfig : EntityTypeConfiguration<VariableQualifier>
    {
        public override void Configure(EntityTypeBuilder<VariableQualifier> builder)
        {
            builder
                .HasKey(t => new { t.QualifierId, t.VariableId });

            builder
               .ToTable("Variable_Qualifiers");

            builder
                .HasOne(dd => dd.Variable)
                .WithMany(d => d.Qualifiers)
                .HasForeignKey(dd => dd.VariableId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(dd => dd.Qualifier)
                .WithMany(d => d.QualifiedVariables)
                .HasForeignKey(dd => dd.QualifierId)
                .OnDelete(DeleteBehavior.Restrict);
               
        }
    }
}
