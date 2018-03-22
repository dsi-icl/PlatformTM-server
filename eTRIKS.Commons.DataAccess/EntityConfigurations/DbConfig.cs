using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eTRIKS.Commons.DataAccess.EntityConfigurations
{
    public class DbConfig : EntityTypeConfiguration<DB>
    {
        public override void Configure(EntityTypeBuilder<DB> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Id)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Name)
                .HasMaxLength(200);

            builder.Property(t => t.UrlPrefix)
                .HasMaxLength(2000);

            builder.Property(t => t.Url)
                .HasMaxLength(2000);

            builder.Property(t => t.Version)
               .HasMaxLength(2000);

            // Table & Column Mappings
            builder.ToTable("Dbs");
            builder.Property(t => t.Id).HasColumnName("OID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.UrlPrefix).HasColumnName("URLPrefix");
            builder.Property(t => t.Url).HasColumnName("URL");
            builder.Property(t => t.Version).HasColumnName("Version");
        }
    }
}
