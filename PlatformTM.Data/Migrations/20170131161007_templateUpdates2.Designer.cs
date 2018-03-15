using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PlatformTM.Data.Migrations
{
    [DbContext(typeof(BioSPEAKdbContext))]
    [Migration("20170131161007_templateUpdates2")]
    partial class templateUpdates2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("eTRIKS.Commons.Core.Application.AccountManagement.UserAccount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("UserAccountId");

                    b.Property<bool>("AdminApproved");

                    b.Property<bool>("EmailConfirmed");

                    b.Property<DateTime>("JoinDate");

                    b.Property<string>("PSK");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<Guid>("UserId");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserAccounts");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Application.AccountManagement.UserClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CalimId");

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserAccountClaims");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Activity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ActivityId");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Name")
                        .HasMaxLength(2000);

                    b.Property<int>("ProjectId")
                        .HasColumnName("ProjectId");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Activities");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Activity");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Biosample", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("BioSampleDBId");

                    b.Property<int>("AssayId");

                    b.Property<string>("BiosampleStudyId");

                    b.Property<int>("DatasetId");

                    b.Property<bool?>("IsBaseline");

                    b.Property<int>("StudyId");

                    b.Property<string>("SubjectId");

                    b.Property<int?>("VisitId");

                    b.HasKey("Id");

                    b.HasIndex("AssayId");

                    b.HasIndex("DatasetId");

                    b.HasIndex("StudyId");

                    b.HasIndex("SubjectId");

                    b.HasIndex("VisitId");

                    b.ToTable("BioSamples");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Characteristic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CharacterisitcId");

                    b.Property<string>("CVtermId");

                    b.Property<int>("CharacteristicFeatureId");

                    b.Property<string>("ControlledValueStr");

                    b.Property<int?>("DatafileId");

                    b.Property<string>("DatasetDomainCode");

                    b.Property<int>("DatasetId");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("VerbatimName")
                        .HasColumnName("CharacObjName")
                        .HasMaxLength(2000);

                    b.Property<string>("VerbatimValue");

                    b.HasKey("Id");

                    b.HasIndex("CVtermId");

                    b.HasIndex("CharacteristicFeatureId");

                    b.HasIndex("DatafileId")
                        .IsUnique();

                    b.HasIndex("DatasetId")
                        .IsUnique();

                    b.ToTable("Characteristics");

                    b.HasDiscriminator<string>("Discriminator");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.CharacteristicFeature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CharacteristicObjId");

                    b.Property<string>("CVtermId");

                    b.Property<string>("DataType");

                    b.Property<string>("Domain");

                    b.Property<string>("FullName");

                    b.Property<int>("ProjectId");

                    b.Property<string>("ShortName");

                    b.HasKey("Id");

                    b.HasIndex("CVtermId");

                    b.HasIndex("ProjectId");

                    b.ToTable("CharacteristicObjects");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.CVterm", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("OID")
                        .HasMaxLength(200);

                    b.Property<string>("Code")
                        .HasMaxLength(200);

                    b.Property<string>("Definition")
                        .HasMaxLength(2000);

                    b.Property<string>("DictionaryId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<bool?>("IsUserSpecified");

                    b.Property<string>("Name")
                        .HasMaxLength(2000);

                    b.Property<string>("Synonyms")
                        .HasMaxLength(2000);

                    b.Property<string>("XrefId")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("DictionaryId");

                    b.HasIndex("XrefId");

                    b.ToTable("CVterms");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.DB", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("OID")
                        .HasMaxLength(200);

                    b.Property<string>("Name")
                        .HasColumnName("Name")
                        .HasMaxLength(200);

                    b.Property<string>("Url")
                        .HasColumnName("URL")
                        .HasMaxLength(2000);

                    b.Property<string>("UrlPrefix")
                        .HasColumnName("URLPrefix")
                        .HasMaxLength(2000);

                    b.Property<string>("Version")
                        .HasColumnName("Version")
                        .HasMaxLength(2000);

                    b.HasKey("Id");

                    b.ToTable("Dbs");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.Dbxref", b =>
                {
                    b.Property<string>("OID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("OID")
                        .HasMaxLength(200);

                    b.Property<string>("Accession")
                        .HasMaxLength(200);

                    b.Property<string>("DBId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("Description")
                        .HasMaxLength(2000);

                    b.HasKey("OID");

                    b.HasIndex("DBId");

                    b.ToTable("DBxreferences");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.Dictionary", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("OID")
                        .HasMaxLength(200);

                    b.Property<string>("Definition")
                        .HasMaxLength(2000);

                    b.Property<string>("Name")
                        .HasMaxLength(2000);

                    b.Property<string>("XrefId")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("XrefId");

                    b.ToTable("Dictionaries");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.DatasetModel.DataFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("DataFileId");

                    b.Property<string>("DataType");

                    b.Property<string>("DateAdded");

                    b.Property<string>("FileName");

                    b.Property<bool>("IsDirectory");

                    b.Property<bool>("IsStandard");

                    b.Property<string>("LastModified");

                    b.Property<bool>("IsLoadedToDB");

                    b.Property<string>("Path");

                    b.Property<int>("ProjectId");

                    b.Property<string>("State");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("DataFiles");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.DatasetModel.Dataset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("OID");

                    b.Property<int>("ActivityId");

                    b.Property<string>("State");

                    b.Property<string>("TemplateId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("TemplateId");

                    b.ToTable("Datasets");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.DatasetModel.VariableDefinition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("OID");

                    b.Property<string>("Accession");

                    b.Property<bool>("AllowMultipleValues");

                    b.Property<string>("CVTermReferenceSourceId");

                    b.Property<string>("CVtermDictionaryId");

                    b.Property<string>("ComputedVarExpression");

                    b.Property<string>("DataType")
                        .HasMaxLength(200);

                    b.Property<string>("Description")
                        .HasMaxLength(2000);

                    b.Property<bool?>("IsComputed")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsCurated")
                        .HasColumnType("bit");

                    b.Property<bool>("IsGeneric");

                    b.Property<string>("Label");

                    b.Property<string>("Name")
                        .HasMaxLength(2000);

                    b.Property<string>("NameQualifier");

                    b.Property<int>("ProjectId");

                    b.Property<string>("RoleId")
                        .HasMaxLength(200);

                    b.Property<string>("Section");

                    b.Property<string>("VariableTypeId")
                        .HasMaxLength(200);

                    b.Property<string>("VariableTypeStr");

                    b.HasKey("Id");

                    b.HasIndex("CVTermReferenceSourceId");

                    b.HasIndex("CVtermDictionaryId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("RoleId")
                        .IsUnique();

                    b.HasIndex("VariableTypeId")
                        .IsUnique();

                    b.ToTable("VariableDefinitions");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.DatasetModel.VariableReference", b =>
                {
                    b.Property<int>("VariableDefinitionId")
                        .HasColumnName("VariableId");

                    b.Property<int>("DatasetId")
                        .HasColumnName("ActivityDatasetId");

                    b.Property<bool?>("IsRequired");

                    b.Property<int?>("KeySequence");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("VariableDefinitionId", "DatasetId");

                    b.HasIndex("DatasetId");

                    b.ToTable("VariableReferences");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.DesignElements.Arm", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ArmId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnName("ArmCode");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("ArmName")
                        .HasMaxLength(2000);

                    b.HasKey("Id");

                    b.ToTable("Arms");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.DesignElements.Visit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("VisitId");

                    b.Property<string>("Name")
                        .HasColumnName("Name")
                        .HasMaxLength(2000);

                    b.Property<int?>("Number");

                    b.Property<int>("StudyDayId");

                    b.Property<int>("StudyId")
                        .HasColumnName("StudyId");

                    b.HasKey("Id");

                    b.HasIndex("StudyDayId")
                        .IsUnique();

                    b.HasIndex("StudyId");

                    b.ToTable("Visits");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.HumanSubject", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("SubjectDBId");

                    b.Property<string>("Arm");

                    b.Property<string>("ArmCode");

                    b.Property<int>("DatasetId");

                    b.Property<string>("StudyArmId");

                    b.Property<int>("StudyId");

                    b.Property<DateTime>("SubjectEndDate");

                    b.Property<DateTime>("SubjectStartDate");

                    b.Property<string>("SubjectStudyId");

                    b.Property<string>("UniqueSubjectId");

                    b.HasKey("Id");

                    b.HasIndex("DatasetId")
                        .IsUnique();

                    b.HasIndex("StudyArmId")
                        .IsUnique();

                    b.HasIndex("StudyId");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Observation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ObservationId");

                    b.Property<string>("Class");

                    b.Property<string>("ControlledTermId");

                    b.Property<string>("ControlledTermStr");

                    b.Property<int?>("DatafileId");

                    b.Property<int?>("DatasetId");

                    b.Property<int?>("DefaultQualifierId")
                        .IsRequired();

                    b.Property<string>("DomainCode");

                    b.Property<string>("DomainName");

                    b.Property<string>("Group");

                    b.Property<string>("Name");

                    b.Property<int?>("ProjectId");

                    b.Property<string>("Subgroup");

                    b.Property<int>("TopicVariableId");

                    b.Property<bool?>("isSubjCharacteristic");

                    b.HasKey("Id");

                    b.HasIndex("ControlledTermId")
                        .IsUnique();

                    b.HasIndex("DatafileId");

                    b.HasIndex("DatasetId");

                    b.HasIndex("DefaultQualifierId")
                        .IsUnique();

                    b.HasIndex("ProjectId");

                    b.HasIndex("TopicVariableId")
                        .IsUnique();

                    b.ToTable("Observations");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ProjectId");

                    b.Property<string>("Accession");

                    b.Property<string>("Description");

                    b.Property<bool>("IsPublic");

                    b.Property<string>("Name");

                    b.Property<Guid>("OwnerId");

                    b.Property<string>("Title");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Study", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("StudyId");

                    b.Property<string>("Accession");

                    b.Property<string>("Description")
                        .HasMaxLength(2000);

                    b.Property<string>("Name")
                        .HasMaxLength(2000);

                    b.Property<int>("ProjectId");

                    b.Property<string>("Site");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Studies");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Templates.DatasetTemplate", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("OID")
                        .HasMaxLength(200);

                    b.Property<string>("Class")
                        .HasMaxLength(200);

                    b.Property<string>("Code")
                        .HasMaxLength(200);

                    b.Property<string>("Description")
                        .HasMaxLength(2000);

                    b.Property<string>("Domain")
                        .HasMaxLength(2000);

                    b.Property<bool>("IsRepeating");

                    b.Property<string>("Structure")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("DomainTemplates");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Templates.DatasetTemplateField", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("OID")
                        .HasMaxLength(200);

                    b.Property<bool>("AllowMultipleValues");

                    b.Property<string>("AllowableQualifiersId");

                    b.Property<string>("ControlledVocabularyId")
                        .HasColumnName("DictionaryId")
                        .HasMaxLength(200);

                    b.Property<string>("DataType")
                        .HasMaxLength(200);

                    b.Property<string>("Description")
                        .HasMaxLength(2000);

                    b.Property<bool>("IsGeneric");

                    b.Property<string>("Label")
                        .HasMaxLength(2000);

                    b.Property<string>("Name")
                        .HasMaxLength(2000);

                    b.Property<int>("Order");

                    b.Property<string>("RoleId")
                        .HasColumnName("RoleTermId")
                        .HasMaxLength(200);

                    b.Property<string>("Section");

                    b.Property<string>("TemplateId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("UsageId")
                        .HasColumnName("UsageTermId")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("AllowableQualifiersId");

                    b.HasIndex("ControlledVocabularyId");

                    b.HasIndex("RoleId");

                    b.HasIndex("TemplateId");

                    b.HasIndex("UsageId");

                    b.ToTable("DomainTemplateVariables");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Timing.TimePoint", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Name")
                        .HasMaxLength(2000);

                    b.Property<int?>("Number");

                    b.Property<int?>("VisitId");

                    b.HasKey("Id");

                    b.HasIndex("VisitId");

                    b.ToTable("Timepoints");

                    b.HasDiscriminator<string>("Discriminator");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("Organization");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.JoinEntities.DatasetDatafile", b =>
                {
                    b.Property<int>("DatasetId");

                    b.Property<int>("DatafileId");

                    b.HasKey("DatasetId", "DatafileId");

                    b.HasIndex("DatafileId");

                    b.ToTable("Dataset_DataFiles");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.JoinEntities.ObservationQualifier", b =>
                {
                    b.Property<int>("ObservationId");

                    b.Property<int>("QualifierId");

                    b.HasKey("ObservationId", "QualifierId");

                    b.HasIndex("QualifierId");

                    b.ToTable("Observation_Qualfiers");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.JoinEntities.ObservationSynonym", b =>
                {
                    b.Property<int>("ObservationId");

                    b.Property<int>("QualifierId");

                    b.HasKey("ObservationId", "QualifierId");

                    b.HasIndex("QualifierId");

                    b.ToTable("Observation_Synonyms");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.JoinEntities.ObservationTiming", b =>
                {
                    b.Property<int>("ObservationId");

                    b.Property<int>("QualifierId");

                    b.HasKey("ObservationId", "QualifierId");

                    b.HasIndex("QualifierId");

                    b.ToTable("Observation_Timings");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.JoinEntities.ProjectUser", b =>
                {
                    b.Property<int>("ProjectId");

                    b.Property<Guid>("UserId");

                    b.HasKey("ProjectId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("Project_Users");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.JoinEntities.StudyArm", b =>
                {
                    b.Property<string>("ArmId");

                    b.Property<int>("StudyId");

                    b.HasKey("ArmId", "StudyId");

                    b.HasIndex("StudyId");

                    b.ToTable("Study_Arms");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.JoinEntities.StudyDataset", b =>
                {
                    b.Property<int>("DatasetId");

                    b.Property<int>("StudyId");

                    b.HasKey("DatasetId", "StudyId");

                    b.HasIndex("StudyId");

                    b.ToTable("Study_Datasets");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.JoinEntities.TemplateFieldDB", b =>
                {
                    b.Property<string>("TemplateFieldId");

                    b.Property<string>("TermSourceId");

                    b.HasKey("TemplateFieldId", "TermSourceId");

                    b.HasIndex("TermSourceId");

                    b.ToTable("TemplateField_TermSource");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Assay", b =>
                {
                    b.HasBaseType("eTRIKS.Commons.Core.Domain.Model.Activity");

                    b.Property<string>("DesignTypeId");

                    b.Property<string>("MeasurementTypeId");

                    b.Property<string>("PlatformAnnotationId");

                    b.Property<string>("TechnologyPlatformId");

                    b.Property<string>("TechnologyTypeId");

                    b.HasIndex("DesignTypeId");

                    b.HasIndex("MeasurementTypeId");

                    b.HasIndex("TechnologyPlatformId");

                    b.HasIndex("TechnologyTypeId");

                    b.ToTable("Assay");

                    b.HasDiscriminator().HasValue("Assay");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.SampleCharacteristic", b =>
                {
                    b.HasBaseType("eTRIKS.Commons.Core.Domain.Model.Characteristic");

                    b.Property<int>("SampleId");

                    b.HasIndex("SampleId");

                    b.ToTable("SampleCharacteristic");

                    b.HasDiscriminator().HasValue("SampleCharacteristic");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.SubjectCharacteristic", b =>
                {
                    b.HasBaseType("eTRIKS.Commons.Core.Domain.Model.Characteristic");

                    b.Property<string>("SubjectId");

                    b.HasIndex("SubjectId");

                    b.ToTable("SubjectCharacteristic");

                    b.HasDiscriminator().HasValue("SubjectCharacteristic");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Timing.AbsoluteTimePoint", b =>
                {
                    b.HasBaseType("eTRIKS.Commons.Core.Domain.Model.Timing.TimePoint");

                    b.Property<DateTime>("DateTime");

                    b.ToTable("AbsoluteTimePoint");

                    b.HasDiscriminator().HasValue("AbsoluteTimePoint");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Timing.RelativeTimePoint", b =>
                {
                    b.HasBaseType("eTRIKS.Commons.Core.Domain.Model.Timing.TimePoint");

                    b.Property<int?>("ReferenceTimePointId");

                    b.HasIndex("ReferenceTimePointId");

                    b.ToTable("RelativeTimePoint");

                    b.HasDiscriminator().HasValue("RelativeTimePoint");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Application.AccountManagement.UserAccount", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Application.AccountManagement.UserClaim", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Application.AccountManagement.UserAccount", "User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Activity", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Project", "Project")
                        .WithMany("Activities")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Biosample", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Assay", "Assay")
                        .WithMany("Biosamples")
                        .HasForeignKey("AssayId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DatasetModel.Dataset", "Dataset")
                        .WithMany()
                        .HasForeignKey("DatasetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Study", "Study")
                        .WithMany()
                        .HasForeignKey("StudyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.HumanSubject", "Subject")
                        .WithMany()
                        .HasForeignKey("SubjectId");

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DesignElements.Visit", "Visit")
                        .WithMany()
                        .HasForeignKey("VisitId");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Characteristic", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.CVterm", "ControlledValue")
                        .WithMany()
                        .HasForeignKey("CVtermId");

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.CharacteristicFeature", "CharacteristicFeature")
                        .WithMany()
                        .HasForeignKey("CharacteristicFeatureId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DatasetModel.DataFile", "Datafile")
                        .WithOne()
                        .HasForeignKey("eTRIKS.Commons.Core.Domain.Model.Characteristic", "DatafileId")
                        .HasConstraintName("FK_Characteristic_DataFile");

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DatasetModel.Dataset", "Dataset")
                        .WithOne()
                        .HasForeignKey("eTRIKS.Commons.Core.Domain.Model.Characteristic", "DatasetId")
                        .HasConstraintName("FK_Characteristic_Dataset")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.CharacteristicFeature", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.CVterm", "ControlledTerm")
                        .WithMany()
                        .HasForeignKey("CVtermId");

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.CVterm", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.Dictionary", "Dictionary")
                        .WithMany("Terms")
                        .HasForeignKey("DictionaryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.Dbxref", "Xref")
                        .WithMany()
                        .HasForeignKey("XrefId");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.Dbxref", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.DB", "DB")
                        .WithMany()
                        .HasForeignKey("DBId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.Dictionary", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.Dbxref", "Xref")
                        .WithMany()
                        .HasForeignKey("XrefId");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.DatasetModel.DataFile", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Project", "Project")
                        .WithMany("DataFiles")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.DatasetModel.Dataset", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Activity", "Activity")
                        .WithMany("Datasets")
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Templates.DatasetTemplate", "Template")
                        .WithMany()
                        .HasForeignKey("TemplateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.DatasetModel.VariableDefinition", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.DB", "CVTermReferenceSource")
                        .WithMany()
                        .HasForeignKey("CVTermReferenceSourceId");

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.Dictionary", "CVtermDictionary")
                        .WithMany()
                        .HasForeignKey("CVtermDictionaryId");

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.CVterm", "Role")
                        .WithOne()
                        .HasForeignKey("eTRIKS.Commons.Core.Domain.Model.DatasetModel.VariableDefinition", "RoleId");

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.CVterm", "VariableType")
                        .WithOne()
                        .HasForeignKey("eTRIKS.Commons.Core.Domain.Model.DatasetModel.VariableDefinition", "VariableTypeId");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.DatasetModel.VariableReference", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DatasetModel.Dataset", "Dataset")
                        .WithMany("Variables")
                        .HasForeignKey("DatasetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DatasetModel.VariableDefinition", "VariableDefinition")
                        .WithMany()
                        .HasForeignKey("VariableDefinitionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.DesignElements.Visit", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Timing.RelativeTimePoint", "StudyDay")
                        .WithOne()
                        .HasForeignKey("eTRIKS.Commons.Core.Domain.Model.DesignElements.Visit", "StudyDayId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Study", "Study")
                        .WithMany("Visits")
                        .HasForeignKey("StudyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.HumanSubject", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DatasetModel.Dataset", "Dataset")
                        .WithOne()
                        .HasForeignKey("eTRIKS.Commons.Core.Domain.Model.HumanSubject", "DatasetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DesignElements.Arm", "StudyArm")
                        .WithOne()
                        .HasForeignKey("eTRIKS.Commons.Core.Domain.Model.HumanSubject", "StudyArmId");

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Study", "Study")
                        .WithMany("Subjects")
                        .HasForeignKey("StudyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Observation", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.CVterm", "ControlledTerm")
                        .WithOne()
                        .HasForeignKey("eTRIKS.Commons.Core.Domain.Model.Observation", "ControlledTermId");

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DatasetModel.DataFile", "Datafile")
                        .WithMany()
                        .HasForeignKey("DatafileId");

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DatasetModel.Dataset", "Dataset")
                        .WithMany()
                        .HasForeignKey("DatasetId");

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DatasetModel.VariableDefinition", "DefaultQualifier")
                        .WithOne()
                        .HasForeignKey("eTRIKS.Commons.Core.Domain.Model.Observation", "DefaultQualifierId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId");

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DatasetModel.VariableDefinition", "TopicVariable")
                        .WithOne()
                        .HasForeignKey("eTRIKS.Commons.Core.Domain.Model.Observation", "TopicVariableId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Project", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Users.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Study", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Project", "Project")
                        .WithMany("Studies")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Templates.DatasetTemplateField", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.Dictionary", "AllowableQualifiers")
                        .WithMany()
                        .HasForeignKey("AllowableQualifiersId");

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.Dictionary", "ControlledVocabulary")
                        .WithMany()
                        .HasForeignKey("ControlledVocabularyId");

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.CVterm", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Templates.DatasetTemplate", "Template")
                        .WithMany("Fields")
                        .HasForeignKey("TemplateId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.CVterm", "Usage")
                        .WithMany()
                        .HasForeignKey("UsageId");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Timing.TimePoint", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DesignElements.Visit")
                        .WithMany("TimePoints")
                        .HasForeignKey("VisitId");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.JoinEntities.DatasetDatafile", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DatasetModel.DataFile", "Datafile")
                        .WithMany("Datasets")
                        .HasForeignKey("DatafileId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DatasetModel.Dataset", "Dataset")
                        .WithMany("DataFiles")
                        .HasForeignKey("DatasetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.JoinEntities.ObservationQualifier", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Observation", "Observation")
                        .WithMany("Qualifiers")
                        .HasForeignKey("ObservationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DatasetModel.VariableDefinition", "Qualifier")
                        .WithMany()
                        .HasForeignKey("QualifierId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.JoinEntities.ObservationSynonym", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Observation", "Observation")
                        .WithMany("Synonyms")
                        .HasForeignKey("ObservationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DatasetModel.VariableDefinition", "Qualifier")
                        .WithMany()
                        .HasForeignKey("QualifierId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.JoinEntities.ObservationTiming", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Observation", "Observation")
                        .WithMany("Timings")
                        .HasForeignKey("ObservationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DatasetModel.VariableDefinition", "Qualifier")
                        .WithMany()
                        .HasForeignKey("QualifierId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.JoinEntities.ProjectUser", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Project", "Project")
                        .WithMany("Users")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Users.User", "User")
                        .WithMany("AffiliatedProjects")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.JoinEntities.StudyArm", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DesignElements.Arm", "Arm")
                        .WithMany("Studies")
                        .HasForeignKey("ArmId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Study", "Study")
                        .WithMany("Arms")
                        .HasForeignKey("StudyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.JoinEntities.StudyDataset", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.DatasetModel.Dataset", "Dataset")
                        .WithMany("Studies")
                        .HasForeignKey("DatasetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Study", "Study")
                        .WithMany("Datasets")
                        .HasForeignKey("StudyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.JoinEntities.TemplateFieldDB", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Templates.DatasetTemplateField", "TemplateField")
                        .WithMany("FieldTermSources")
                        .HasForeignKey("TemplateFieldId")
                        .HasConstraintName("FK_TemplateField_TermSource")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.DB", "TermSource")
                        .WithMany()
                        .HasForeignKey("TermSourceId")
                        .HasConstraintName("FK_TermSource_TemplateField")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Assay", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.CVterm", "DesignType")
                        .WithMany()
                        .HasForeignKey("DesignTypeId");

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.CVterm", "MeasurementType")
                        .WithMany()
                        .HasForeignKey("MeasurementTypeId");

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.CVterm", "TechnologyPlatform")
                        .WithMany()
                        .HasForeignKey("TechnologyPlatformId");

                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.ControlledTerminology.CVterm", "TechnologyType")
                        .WithMany()
                        .HasForeignKey("TechnologyTypeId");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.SampleCharacteristic", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Biosample", "Sample")
                        .WithMany("SampleCharacteristics")
                        .HasForeignKey("SampleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.SubjectCharacteristic", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.HumanSubject", "Subject")
                        .WithMany("SubjectCharacteristics")
                        .HasForeignKey("SubjectId");
                });

            modelBuilder.Entity("eTRIKS.Commons.Core.Domain.Model.Timing.RelativeTimePoint", b =>
                {
                    b.HasOne("eTRIKS.Commons.Core.Domain.Model.Timing.TimePoint", "ReferenceTimePoint")
                        .WithMany()
                        .HasForeignKey("ReferenceTimePointId");
                });
        }
    }
}
