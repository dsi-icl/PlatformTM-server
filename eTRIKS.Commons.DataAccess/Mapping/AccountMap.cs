using System;
using eTRIKS.Commons.Core.Application.AccountManagement;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace eTRIKS.Commons.Persistence.Mapping
{
    internal class AccountMap : EntityTypeConfiguration<Account>
    {
        //public AccountMap()
        //{
        //    this.HasKey(t => t.Id);

        //    //this.Property(t => t.UserName).HasColumnName("Username");
        //    //this.Property(t => t.).HasColumnName("Username");

        //    // Table & Column Mappings
        //    this.ToTable("UserAccounts");
        //    this.Property(t => t.Id).HasColumnName("UserAccountId");
        //}
        public override void Map(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(t => t.Id);

            //this.Property(t => t.UserName).HasColumnName("Username");
            //this.Property(t => t.).HasColumnName("Username");

            // Table & Column Mappings
            builder.ToTable("UserAccounts");
            builder.Property(t => t.Id).HasColumnName("UserAccountId");
        }
    }
}