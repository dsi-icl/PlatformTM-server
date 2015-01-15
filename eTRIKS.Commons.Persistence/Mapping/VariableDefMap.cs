using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class VariableDefMap : EntityTypeConfiguration<VariableDefinition>
    {
        public VariableDefMap()
        {
            // Primary Key
            this.HasKey(t => t.OID);

            // Properties
            this.Property(t => t.OID)
                .IsRequired();
                //.HasMaxLength(200);

            this.Property(t => t.Name)
                .HasMaxLength(2000);

            this.Property(t => t.Description)
                .HasMaxLength(2000);

            this.Property(t => t.DataType)
                .HasMaxLength(200);

            this.Property(t => t.StudyId)
                .HasMaxLength(200);

            this.Property(t => t.VariableTypeId)
                .HasMaxLength(200);

            this.Property(t => t.RoleId)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Variable_Definition_TBL");
            //this.Property(t => t.OID).HasColumnName("OID");
            //this.Property(t => t.Name).HasColumnName("name");
            //this.Property(t => t.Description).HasColumnName("description");
            //this.Property(t => t.DataType).HasColumnName("dataType");
            //this.Property(t => t.StudyId).HasColumnName("studyId");
            //this.Property(t => t.IsCurated).HasColumnName("isCurated");
            //this.Property(t => t.VariableTypeId).HasColumnName("variableType");
            //this.Property(t => t.RoleId).HasColumnName("role");

            this.HasOptional(t => t.Study)
               .WithMany()
               .HasForeignKey(t => t.StudyId);
            
            this.HasOptional(t => t.Role)
                .WithMany()
                .HasForeignKey(t => t.RoleId);

            this.HasOptional(t => t.VariableType)
               .WithMany()
               .HasForeignKey(t => t.VariableTypeId);

            //this.HasOptional(t => t.DerivedVariableProperties)
            //   .WithMany()
            //   .HasForeignKey(t => t.DerivedVariablePropertiesId);
        }
    }
}
