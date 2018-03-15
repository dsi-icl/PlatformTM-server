using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.Application.AccountManagement;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{

    internal class AccountConfig : EntityTypeConfiguration<UserAccount>
    {
        public override void Configure(EntityTypeBuilder<UserAccount> builder)
        {
            builder
                .HasKey(t => t.Id);

            // Table & Column Mappings
            builder.ToTable("UserAccounts");
            builder.Property(t => t.Id).HasColumnName("UserAccountId");

            //builder.HasMany(a=>a.Claims)
        }
    }
}