using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.Application.AccountManagement;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{
    internal class ClaimConfig : EntityTypeConfiguration<UserClaim>
    {
        public override void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.HasKey(t => t.Id);
            // Table & Column Mappings
            builder.ToTable("UserAccountClaims");
            builder.Property(t => t.Id).HasColumnName("CalimId");

           

            builder.HasOne(t => t.UserAccount)
               .WithMany(s => s.Claims)
               .HasForeignKey(t => t.UserAccountId);
        }
    }
}