using eTRIKS.Commons.Core.JoinEntities;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
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
                .WithMany(d => d.VariableQualifiers)
                .HasForeignKey(dd => dd.VariableId);

            builder
                .HasOne(dd => dd.Qualifier)
                .WithMany()
                .HasForeignKey(dd => dd.QualifierId);
                
        }
    }
}
