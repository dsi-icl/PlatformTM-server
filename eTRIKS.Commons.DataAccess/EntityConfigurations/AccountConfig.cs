using System;
using eTRIKS.Commons.Core.Application.AccountManagement;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
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
        }
    }
}