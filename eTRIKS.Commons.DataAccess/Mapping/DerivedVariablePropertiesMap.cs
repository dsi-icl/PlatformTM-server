using System;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.DataAccess.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class DerivedVariablePropertiesMap : EntityTypeConfiguration<DerivedMethod>
    {
        //public DerivedVariablePropertiesMap()
        //{
        //    // Primary Key
        //    this.HasKey(t => t.Id);

        //    // Properties
        //    this.Property(t => t.DerivedVariableId)
        //        .IsRequired()
        //        .HasMaxLength(200);

        //    this.Property(t => t.MethodName)
        //        .HasMaxLength(2000);

        //    this.Property(t => t.MethodDescription)
        //        .HasMaxLength(2000);

        //    this.Property(t => t.DerivedValueTypeId)
        //        .HasMaxLength(200);

        //    // Table & Column Mappings
        //    this.ToTable("Derived_Method_TBL");
        //    this.Property(t => t.Id).HasColumnName("OID");
        //    this.Property(t => t.DerivedVariableId).HasColumnName("derivedVariableId");
        //    this.Property(t => t.MethodDescription).HasColumnName("methodDescription");
        //    this.Property(t => t.DerivedValueTypeId).HasColumnName("type");

        //    // Relationships
        //    //this.HasRequired(t => t.DerivedVariable);

        //}

        public override void Map(EntityTypeBuilder<DerivedMethod> builder)
        {
            throw new NotImplementedException();
        }
    }
}
