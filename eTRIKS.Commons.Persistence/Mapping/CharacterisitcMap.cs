using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model;

namespace eTRIKS.Commons.Persistence.Mapping
{
    class CharacterisitcMap : EntityTypeConfiguration<Characterisitc>
    {
        public CharacterisitcMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            this.Property(t => t.VerbatimName)
                .HasMaxLength(2000);

            //this.Property(t => t.StudyId)
            //    .IsRequired()
            //    .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Characteristics_TBL");
            this.Property(t => t.Id).HasColumnName("CharacterisitcId");
            this.Property(t => t.VerbatimName).HasColumnName("CharacObjName");
            //this.Property(t => t.StudyId).HasColumnName("StudyId");

            // Relationships
            //this.HasOptional(t => t.DatasetVariable).WithRequired()
             //   .WithOptionalDependent().Map(m => m.MapKey("DatasetVariableId","DatasetId"));
            //.HasForeignKey(t => t.DatasetVariableId);
            this.HasRequired(p => p.DatasetVariable)
                .WithMany()
                .HasForeignKey(k => new {k.DatasetVariableId,k.DatasetId});

            //this.HasMany(t => t.Studies)
            //    .WithMany(t => t.Activities)
            //    .Map(mc =>
            //    {
            //        mc.ToTable("Study_Activities");
            //        mc.MapLeftKey("StudyId");
            //        mc.MapRightKey("ActivityId");

            //    });

        }
    }
}
