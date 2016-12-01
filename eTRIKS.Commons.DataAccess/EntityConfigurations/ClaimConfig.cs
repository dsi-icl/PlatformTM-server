using eTRIKS.Commons.Core.Application.AccountManagement;
using eTRIKS.Commons.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    internal class ClaimConfig : EntityTypeConfiguration<UserClaim>
    {
        public override void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.HasKey(t => t.Id);
            // Table & Column Mappings
            builder.ToTable("UserAccountClaims");
            builder.Property(t => t.Id).HasColumnName("CalimId");
        }
    }
}