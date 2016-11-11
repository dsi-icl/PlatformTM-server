using System;
using eTRIKS.Commons.Core.Application.AccountManagement;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.Persistence.Mapping
{
    internal class ClaimMap : EntityTypeConfiguration<Claim>
    {
        //public ClaimMap()
        //{
        //    this.HasKey(t => t.Id);


        //    // Table & Column Mappings
        //    this.ToTable("UserAccountClaims");
        //    this.Property(t => t.Id).HasColumnName("CalimId");
        //}

        public override void Map(EntityTypeBuilder<Claim> builder)
        {
            throw new NotImplementedException();
        }
    }
}