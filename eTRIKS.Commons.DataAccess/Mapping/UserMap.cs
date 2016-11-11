using System;
using eTRIKS.Commons.Core.Domain.Model.Users;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        //public UserMap()
        //{
        //    this.HasKey(t => t.Id);

        //    // Properties
        //    this.Property(t => t.Id)
        //        .IsRequired();

        //    this.ToTable("Users");
        //}

        public override void Map(EntityTypeBuilder<User> builder)
        {
            throw new NotImplementedException();
        }
    }
}