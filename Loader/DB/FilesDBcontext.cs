using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlatformTM.Core.Application.AccountManagement;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.ControlledTerminology;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.DesignElements;
using PlatformTM.Core.Domain.Model.Templates;
using PlatformTM.Core.Domain.Model.Timing;
using PlatformTM.Core.Domain.Model.Users;
using PlatformTM.Core.JoinEntities;

namespace Loader.DB
{
    public class FilesDBcontext : DbContext
    {
        const string connectionString = "server=146.169.11.152;user id=root;password=imperial;persistsecurityinfo=True;database=eTRIKSdata;Allow User Variables=True";
        public DbSet<DataFile> Files { get; set; }
        public DbSet<Project> Projects { get; set; }

        public FilesDBcontext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));

            var serverVersion = ServerVersion.AutoDetect(connectionString);

            optionsBuilder
                .UseMySql(connectionString, serverVersion)
                // The following three options help with debugging, but should
                // be changed or removed for production.
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            

            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<UserAccount>();
            modelBuilder.Ignore<PlatformTM.Core.Domain.Model.Activity>();
            modelBuilder.Ignore<Arm>();
            modelBuilder.Ignore<Assay>();

            modelBuilder.Ignore<Biosample>();
            modelBuilder.Ignore<Characteristic>();
            modelBuilder.Ignore<CharacteristicFeature>();
            modelBuilder.Ignore<UserClaim>();


            modelBuilder.Ignore<DatasetTemplate>();
            modelBuilder.Ignore<DatasetTemplateField>();
            modelBuilder.Ignore<TemplateFieldDB>();
            modelBuilder.Ignore<CVterm>();
            modelBuilder.Ignore<PlatformTM.Core.Domain.Model.ControlledTerminology.DB>();
            modelBuilder.Ignore<Dbxref>();
            modelBuilder.Ignore<Dictionary>();


            modelBuilder.Entity<DataFile>(entity =>
            {
                // Primary Key
                entity.HasKey(t => t.Id);
                // Table & Column Mappings
                entity.ToTable("DataFiles");
                entity.Property(t => t.Id).HasColumnName("DataFileId");
                entity.Property(t => t.FolderId).HasColumnName("ParentId");

                // Relationships
                entity.HasOne(t => t.Project)
                    .WithMany(s => s.DataFiles)
                    .IsRequired()
                    .HasForeignKey(t => t.ProjectId);

                entity.HasOne(t => t.Folder)
                       .WithMany()
                       .HasForeignKey(t => t.FolderId);

            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(t => t.Id);


                // Table & Column Mappings
                entity.ToTable("Projects");
                entity.Property(t => t.Id).HasColumnName("ProjectId");

                entity.Ignore(t => t.Owner);

            });





            modelBuilder.Ignore<Dataset>();
            modelBuilder.Ignore<DatasetDatafile>();

            modelBuilder.Ignore<PlatformTM.Core.Domain.Model.Observation>();
            modelBuilder.Ignore<ObservationQualifier>();
            modelBuilder.Ignore<ObservationSynonym>();
            modelBuilder.Ignore<ObservationTiming>();

            //modelBuilder.AddConfiguration<Project>(new ProjectConfig());
            modelBuilder.Ignore<ProjectUser>();

            modelBuilder.Ignore<Study>();
            modelBuilder.Ignore<StudyDataset>();
            modelBuilder.Ignore<StudyArm>();

            modelBuilder.Ignore<HumanSubject>();

            modelBuilder.Ignore<TimePoint>();

            modelBuilder.Ignore<User>();


            modelBuilder.Ignore<VariableDefinition>();
            modelBuilder.Ignore<VariableReference>();
            modelBuilder.Ignore<VariableQualifier>();

            modelBuilder.Ignore<Visit>();

        }
    }
}

