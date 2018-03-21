using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Data.Extensions;

namespace PlatformTM.Data.EntityConfigurations
{
    public class VariableDefConfig : EntityTypeConfiguration<VariableDefinition>
    {
        //public VariableDefMap()
        //{
        //    //this.HasMany(t => t.VariableQualifiers)
        //    //    .WithMany()
        //    //    .Map(mc =>
        //    //    {
        //    //        mc.ToTable("Variable_Qualifiers");
        //    //        mc.MapLeftKey("VariableId");
        //    //        mc.MapRightKey("QualifierId");

        //    //    });

        //    //this.HasOptional(t => t.DerivedVariableProperties)
        //    //   .WithMany()
        //    //   .HasForeignKey(t => t.DerivedVariablePropertiesId);
        //}

        public override void Configure(EntityTypeBuilder<VariableDefinition> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Id)
                .IsRequired();
            //.HasMaxLength(200);

            builder.Property(t => t.IsCurated).HasColumnType("bit");
            builder.Property(t => t.IsComputed).HasColumnType("bit");
            //builder.Property(t => t.ComputedVarExpression).IsOptional();
            builder.Property(t => t.Name)
                .HasMaxLength(2000);

            builder.Property(t => t.Description)
                .HasMaxLength(2000);

            builder.Property(t => t.DataType)
                .HasMaxLength(200);

            builder.Property(t => t.ProjectId);
            //.HasMaxLength(200);

            builder.Property(t => t.VariableTypeId)
                .HasMaxLength(200);

            builder.Property(t => t.RoleId)
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("VariableDefinitions");
            builder.Property(t => t.Id).HasColumnName("OID");


            builder.HasOne(t => t.Project)
               .WithMany()
               .IsRequired()
               .HasForeignKey(t => t.ProjectId);

            builder.HasOne(t => t.Role)
                   .WithMany()
                   .HasForeignKey(t => t.RoleId);

            builder.HasOne(t => t.VariableType)
                   .WithMany()
                   .HasForeignKey(t => t.VariableTypeId);
        }
    }
}
