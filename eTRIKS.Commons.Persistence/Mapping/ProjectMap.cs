using System.Data.Entity.ModelConfiguration;
using eTRIKS.Commons.Core.Domain.Model;

namespace eTRIKS.Commons.Persistence.Mapping
{
    public class ProjectMap : EntityTypeConfiguration<Project>
    {
        public ProjectMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            
            // Table & Column Mappings
            this.ToTable("Project_TBL");
            this.Property(t => t.Id).HasColumnName("ProjectId");

           
        }
    }
}
