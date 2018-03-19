using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PlatformTM.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dbs",
                columns: table => new
                {
                    OID = table.Column<string>(maxLength: 200, nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    URL = table.Column<string>(maxLength: 2000, nullable: true),
                    URLPrefix = table.Column<string>(maxLength: 2000, nullable: true),
                    Version = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dbs", x => x.OID);
                });

            migrationBuilder.CreateTable(
                name: "Arms",
                columns: table => new
                {
                    ArmId = table.Column<string>(nullable: false),
                    ArmCode = table.Column<string>(nullable: false),
                    ArmName = table.Column<string>(maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arms", x => x.ArmId);
                });

            migrationBuilder.CreateTable(
                name: "DomainTemplates",
                columns: table => new
                {
                    OID = table.Column<string>(maxLength: 200, nullable: false),
                    Class = table.Column<string>(maxLength: 200, nullable: true),
                    Code = table.Column<string>(maxLength: 200, nullable: true),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Domain = table.Column<string>(maxLength: 2000, nullable: true),
                    IsRepeating = table.Column<bool>(nullable: false),
                    Structure = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainTemplates", x => x.OID);
                });

            migrationBuilder.CreateTable(
                name: "TimePoints",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Discriminator = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 2000, nullable: true),
                    Number = table.Column<int>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: true),
                    ReferenceTimePointId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimePoints_TimePoints_ReferenceTimePointId",
                        column: x => x.ReferenceTimePointId,
                        principalTable: "TimePoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Organization = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DBxreferences",
                columns: table => new
                {
                    OID = table.Column<string>(maxLength: 200, nullable: false),
                    Accession = table.Column<string>(maxLength: 200, nullable: true),
                    DBId = table.Column<string>(maxLength: 200, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBxreferences", x => x.OID);
                    table.ForeignKey(
                        name: "FK_DBxreferences_Dbs_DBId",
                        column: x => x.DBId,
                        principalTable: "Dbs",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAccounts",
                columns: table => new
                {
                    UserAccountId = table.Column<Guid>(nullable: false),
                    AdminApproved = table.Column<bool>(nullable: false),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    JoinDate = table.Column<DateTime>(nullable: false),
                    PSK = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccounts", x => x.UserAccountId);
                    table.ForeignKey(
                        name: "FK_UserAccounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Accession = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsPublic = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OwnerId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                    table.ForeignKey(
                        name: "FK_Projects_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dictionaries",
                columns: table => new
                {
                    OID = table.Column<string>(maxLength: 200, nullable: false),
                    Definition = table.Column<string>(maxLength: 2000, nullable: true),
                    Name = table.Column<string>(maxLength: 2000, nullable: true),
                    XrefId = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictionaries", x => x.OID);
                    table.ForeignKey(
                        name: "FK_Dictionaries_DBxreferences_XrefId",
                        column: x => x.XrefId,
                        principalTable: "DBxreferences",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAccountClaims",
                columns: table => new
                {
                    CalimId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserAccountId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccountClaims", x => x.CalimId);
                    table.ForeignKey(
                        name: "FK_UserAccountClaims_UserAccounts_UserAccountId",
                        column: x => x.UserAccountId,
                        principalTable: "UserAccounts",
                        principalColumn: "UserAccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataFiles",
                columns: table => new
                {
                    DataFileId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    DataType = table.Column<string>(nullable: true),
                    DateAdded = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    Format = table.Column<string>(nullable: true),
                    IsDirectory = table.Column<bool>(nullable: false),
                    IsLoadedToDB = table.Column<bool>(nullable: false),
                    IsStandard = table.Column<bool>(nullable: false),
                    LastModified = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    ProjectId = table.Column<int>(nullable: false),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataFiles", x => x.DataFileId);
                    table.ForeignKey(
                        name: "FK_DataFiles_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Studies",
                columns: table => new
                {
                    StudyId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Accession = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Name = table.Column<string>(maxLength: 2000, nullable: true),
                    ProjectId = table.Column<int>(nullable: false),
                    Site = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studies", x => x.StudyId);
                    table.ForeignKey(
                        name: "FK_Studies_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Project_Users",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project_Users", x => new { x.ProjectId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Project_Users_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Project_Users_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CVterms",
                columns: table => new
                {
                    OID = table.Column<string>(maxLength: 200, nullable: false),
                    Code = table.Column<string>(maxLength: 200, nullable: true),
                    Definition = table.Column<string>(maxLength: 2000, nullable: true),
                    DictionaryId = table.Column<string>(maxLength: 200, nullable: false),
                    IsUserSpecified = table.Column<bool>(nullable: true),
                    Name = table.Column<string>(maxLength: 2000, nullable: true),
                    Synonyms = table.Column<string>(maxLength: 2000, nullable: true),
                    XrefId = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CVterms", x => x.OID);
                    table.ForeignKey(
                        name: "FK_CVterms_Dictionaries_DictionaryId",
                        column: x => x.DictionaryId,
                        principalTable: "Dictionaries",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CVterms_DBxreferences_XrefId",
                        column: x => x.XrefId,
                        principalTable: "DBxreferences",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Visits",
                columns: table => new
                {
                    VisitId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Name = table.Column<string>(maxLength: 2000, nullable: true),
                    Number = table.Column<int>(nullable: true),
                    StudyDayId = table.Column<int>(nullable: false),
                    StudyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visits", x => x.VisitId);
                    table.ForeignKey(
                        name: "FK_Visits_TimePoints_StudyDayId",
                        column: x => x.StudyDayId,
                        principalTable: "TimePoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Visits_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "StudyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Study_Arms",
                columns: table => new
                {
                    ArmId = table.Column<string>(nullable: false),
                    StudyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Study_Arms", x => new { x.ArmId, x.StudyId });
                    table.ForeignKey(
                        name: "FK_Study_Arms_Arms_ArmId",
                        column: x => x.ArmId,
                        principalTable: "Arms",
                        principalColumn: "ArmId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Study_Arms_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "StudyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    ActivityId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Discriminator = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 2000, nullable: true),
                    ProjectId = table.Column<int>(nullable: false),
                    DesignTypeId = table.Column<string>(nullable: true),
                    HasTemporalData = table.Column<bool>(nullable: true),
                    MeasurementTypeId = table.Column<string>(nullable: true),
                    PlatformAnnotationId = table.Column<string>(nullable: true),
                    TechnologyPlatformId = table.Column<string>(nullable: true),
                    TechnologyTypeId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.ActivityId);
                    table.ForeignKey(
                        name: "FK_Activities_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Activities_CVterms_DesignTypeId",
                        column: x => x.DesignTypeId,
                        principalTable: "CVterms",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activities_CVterms_MeasurementTypeId",
                        column: x => x.MeasurementTypeId,
                        principalTable: "CVterms",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activities_CVterms_TechnologyPlatformId",
                        column: x => x.TechnologyPlatformId,
                        principalTable: "CVterms",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activities_CVterms_TechnologyTypeId",
                        column: x => x.TechnologyTypeId,
                        principalTable: "CVterms",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VariableDefinitions",
                columns: table => new
                {
                    OID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Accession = table.Column<string>(nullable: true),
                    AllowMultipleValues = table.Column<bool>(nullable: false),
                    CVTermReferenceSourceId = table.Column<string>(nullable: true),
                    CVtermDictionaryId = table.Column<string>(nullable: true),
                    ComputedVarExpression = table.Column<string>(nullable: true),
                    DataType = table.Column<string>(maxLength: 200, nullable: true),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    IsComputed = table.Column<bool>(type: "bit", nullable: true),
                    IsCurated = table.Column<bool>(type: "bit", nullable: true),
                    IsGeneric = table.Column<bool>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 2000, nullable: true),
                    NameQualifier = table.Column<string>(nullable: true),
                    ProjectId = table.Column<int>(nullable: false),
                    RoleId = table.Column<string>(maxLength: 200, nullable: true),
                    Section = table.Column<string>(nullable: true),
                    VariableTypeId = table.Column<string>(maxLength: 200, nullable: true),
                    VariableTypeStr = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariableDefinitions", x => x.OID);
                    table.ForeignKey(
                        name: "FK_VariableDefinitions_Dbs_CVTermReferenceSourceId",
                        column: x => x.CVTermReferenceSourceId,
                        principalTable: "Dbs",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VariableDefinitions_Dictionaries_CVtermDictionaryId",
                        column: x => x.CVtermDictionaryId,
                        principalTable: "Dictionaries",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VariableDefinitions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VariableDefinitions_CVterms_RoleId",
                        column: x => x.RoleId,
                        principalTable: "CVterms",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VariableDefinitions_CVterms_VariableTypeId",
                        column: x => x.VariableTypeId,
                        principalTable: "CVterms",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DomainTemplateVariables",
                columns: table => new
                {
                    OID = table.Column<string>(maxLength: 200, nullable: false),
                    AllowMultipleValues = table.Column<bool>(nullable: false),
                    CVTermsDictionaryId = table.Column<string>(maxLength: 200, nullable: true),
                    DataType = table.Column<string>(maxLength: 200, nullable: true),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    IsGeneric = table.Column<bool>(nullable: false),
                    Label = table.Column<string>(maxLength: 2000, nullable: true),
                    Name = table.Column<string>(maxLength: 2000, nullable: true),
                    Order = table.Column<int>(nullable: false),
                    QualifiersDictionaryId = table.Column<string>(nullable: true),
                    RoleTermId = table.Column<string>(maxLength: 200, nullable: true),
                    Section = table.Column<string>(nullable: true),
                    TemplateId = table.Column<string>(maxLength: 200, nullable: false),
                    UsageTermId = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainTemplateVariables", x => x.OID);
                    table.ForeignKey(
                        name: "FK_DomainTemplateVariables_Dictionaries_CVTermsDictionaryId",
                        column: x => x.CVTermsDictionaryId,
                        principalTable: "Dictionaries",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DomainTemplateVariables_Dictionaries_QualifiersDictionaryId",
                        column: x => x.QualifiersDictionaryId,
                        principalTable: "Dictionaries",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DomainTemplateVariables_CVterms_RoleTermId",
                        column: x => x.RoleTermId,
                        principalTable: "CVterms",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DomainTemplateVariables_DomainTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "DomainTemplates",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DomainTemplateVariables_CVterms_UsageTermId",
                        column: x => x.UsageTermId,
                        principalTable: "CVterms",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacteristicObjects",
                columns: table => new
                {
                    CharacteristicObjId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    ActivityId = table.Column<int>(nullable: true),
                    CVtermId = table.Column<string>(nullable: true),
                    DataType = table.Column<string>(nullable: true),
                    Domain = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    ProjectId = table.Column<int>(nullable: false),
                    ShortName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacteristicObjects", x => x.CharacteristicObjId);
                    table.ForeignKey(
                        name: "FK_CharacteristicObjects_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "ActivityId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacteristicObjects_CVterms_CVtermId",
                        column: x => x.CVtermId,
                        principalTable: "CVterms",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacteristicObjects_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Datasets",
                columns: table => new
                {
                    OID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    ActivityId = table.Column<int>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    TemplateId = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Datasets", x => x.OID);
                    table.ForeignKey(
                        name: "FK_Datasets_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "ActivityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Datasets_DomainTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "DomainTemplates",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateField_TermSource",
                columns: table => new
                {
                    TemplateFieldId = table.Column<string>(nullable: false),
                    TermSourceId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateField_TermSource", x => new { x.TemplateFieldId, x.TermSourceId });
                    table.ForeignKey(
                        name: "FK_TemplateField_TermSource",
                        column: x => x.TemplateFieldId,
                        principalTable: "DomainTemplateVariables",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TermSource_TemplateField",
                        column: x => x.TermSourceId,
                        principalTable: "Dbs",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VariableReferences",
                columns: table => new
                {
                    VariableId = table.Column<int>(nullable: false),
                    ActivityDatasetId = table.Column<int>(nullable: false),
                    IsRequired = table.Column<bool>(nullable: true),
                    KeySequence = table.Column<int>(nullable: true),
                    OrderNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariableReferences", x => new { x.VariableId, x.ActivityDatasetId });
                    table.ForeignKey(
                        name: "FK_VariableReferences_Datasets_ActivityDatasetId",
                        column: x => x.ActivityDatasetId,
                        principalTable: "Datasets",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VariableReferences_VariableDefinitions_VariableId",
                        column: x => x.VariableId,
                        principalTable: "VariableDefinitions",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    SubjectDBId = table.Column<string>(nullable: false),
                    Arm = table.Column<string>(nullable: true),
                    ArmCode = table.Column<string>(nullable: true),
                    DatafileId = table.Column<int>(nullable: false),
                    DatasetId = table.Column<int>(nullable: false),
                    StudyArmId = table.Column<string>(nullable: true),
                    StudyId = table.Column<int>(nullable: false),
                    SubjectEndDate = table.Column<DateTime>(nullable: false),
                    SubjectStartDate = table.Column<DateTime>(nullable: false),
                    SubjectStudyId = table.Column<string>(nullable: true),
                    UniqueSubjectId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.SubjectDBId);
                    table.ForeignKey(
                        name: "FK_Subjects_DataFiles_DatafileId",
                        column: x => x.DatafileId,
                        principalTable: "DataFiles",
                        principalColumn: "DataFileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subjects_Datasets_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Datasets",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subjects_Arms_StudyArmId",
                        column: x => x.StudyArmId,
                        principalTable: "Arms",
                        principalColumn: "ArmId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subjects_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "StudyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Observations",
                columns: table => new
                {
                    ObservationId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Class = table.Column<string>(nullable: true),
                    ControlledTermId = table.Column<string>(nullable: true),
                    ControlledTermStr = table.Column<string>(nullable: true),
                    DatafileId = table.Column<int>(nullable: true),
                    DatasetId = table.Column<int>(nullable: true),
                    DefaultQualifierId = table.Column<int>(nullable: false),
                    DomainCode = table.Column<string>(nullable: true),
                    DomainName = table.Column<string>(nullable: true),
                    Group = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    Subgroup = table.Column<string>(nullable: true),
                    TopicVariableId = table.Column<int>(nullable: false),
                    isSubjCharacteristic = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observations", x => x.ObservationId);
                    table.ForeignKey(
                        name: "FK_Observations_CVterms_ControlledTermId",
                        column: x => x.ControlledTermId,
                        principalTable: "CVterms",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Observations_DataFiles_DatafileId",
                        column: x => x.DatafileId,
                        principalTable: "DataFiles",
                        principalColumn: "DataFileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Observations_Datasets_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Datasets",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Observations_VariableDefinitions_DefaultQualifierId",
                        column: x => x.DefaultQualifierId,
                        principalTable: "VariableDefinitions",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Observations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Observations_VariableDefinitions_TopicVariableId",
                        column: x => x.TopicVariableId,
                        principalTable: "VariableDefinitions",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dataset_DataFiles",
                columns: table => new
                {
                    DatasetId = table.Column<int>(nullable: false),
                    DatafileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dataset_DataFiles", x => new { x.DatasetId, x.DatafileId });
                    table.ForeignKey(
                        name: "FK_Dataset_DataFiles_DataFiles_DatafileId",
                        column: x => x.DatafileId,
                        principalTable: "DataFiles",
                        principalColumn: "DataFileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dataset_DataFiles_Datasets_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Datasets",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Study_Datasets",
                columns: table => new
                {
                    DatasetId = table.Column<int>(nullable: false),
                    StudyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Study_Datasets", x => new { x.DatasetId, x.StudyId });
                    table.ForeignKey(
                        name: "FK_Study_Datasets_Datasets_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Datasets",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Study_Datasets_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "StudyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BioSamples",
                columns: table => new
                {
                    BioSampleDBId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    AssayId = table.Column<int>(nullable: false),
                    BiosampleStudyId = table.Column<string>(nullable: true),
                    CollectionDateTime = table.Column<DateTime>(nullable: false),
                    CollectionStudyDayId = table.Column<int>(nullable: true),
                    CollectionStudyTimePointId = table.Column<int>(nullable: true),
                    DataFileId = table.Column<int>(nullable: true),
                    DatasetId = table.Column<int>(nullable: false),
                    IsBaseline = table.Column<bool>(nullable: true),
                    StudyId = table.Column<int>(nullable: false),
                    SubjectId = table.Column<string>(nullable: true),
                    VisitId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BioSamples", x => x.BioSampleDBId);
                    table.ForeignKey(
                        name: "FK_BioSamples_Activities_AssayId",
                        column: x => x.AssayId,
                        principalTable: "Activities",
                        principalColumn: "ActivityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BioSamples_TimePoints_CollectionStudyDayId",
                        column: x => x.CollectionStudyDayId,
                        principalTable: "TimePoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BioSamples_TimePoints_CollectionStudyTimePointId",
                        column: x => x.CollectionStudyTimePointId,
                        principalTable: "TimePoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BioSamples_DataFiles_DataFileId",
                        column: x => x.DataFileId,
                        principalTable: "DataFiles",
                        principalColumn: "DataFileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BioSamples_Datasets_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Datasets",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BioSamples_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "StudyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BioSamples_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "SubjectDBId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BioSamples_Visits_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visits",
                        principalColumn: "VisitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Observation_Qualfiers",
                columns: table => new
                {
                    ObservationId = table.Column<int>(nullable: false),
                    QualifierId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observation_Qualfiers", x => new { x.ObservationId, x.QualifierId });
                    table.ForeignKey(
                        name: "FK_Observation_Qualfiers_Observations_ObservationId",
                        column: x => x.ObservationId,
                        principalTable: "Observations",
                        principalColumn: "ObservationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Observation_Qualfiers_VariableDefinitions_QualifierId",
                        column: x => x.QualifierId,
                        principalTable: "VariableDefinitions",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Observation_Synonyms",
                columns: table => new
                {
                    ObservationId = table.Column<int>(nullable: false),
                    QualifierId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observation_Synonyms", x => new { x.ObservationId, x.QualifierId });
                    table.ForeignKey(
                        name: "FK_Observation_Synonyms_Observations_ObservationId",
                        column: x => x.ObservationId,
                        principalTable: "Observations",
                        principalColumn: "ObservationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Observation_Synonyms_VariableDefinitions_QualifierId",
                        column: x => x.QualifierId,
                        principalTable: "VariableDefinitions",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Observation_Timings",
                columns: table => new
                {
                    ObservationId = table.Column<int>(nullable: false),
                    QualifierId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observation_Timings", x => new { x.ObservationId, x.QualifierId });
                    table.ForeignKey(
                        name: "FK_Observation_Timings_Observations_ObservationId",
                        column: x => x.ObservationId,
                        principalTable: "Observations",
                        principalColumn: "ObservationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Observation_Timings_VariableDefinitions_QualifierId",
                        column: x => x.QualifierId,
                        principalTable: "VariableDefinitions",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Characteristics",
                columns: table => new
                {
                    CharacteristicId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    CVtermId = table.Column<string>(nullable: true),
                    CharacteristicFeatureId = table.Column<int>(nullable: false),
                    ControlledValueStr = table.Column<string>(nullable: true),
                    DatafileId = table.Column<int>(nullable: true),
                    DatasetId = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    CharacObjName = table.Column<string>(maxLength: 2000, nullable: true),
                    VerbatimValue = table.Column<string>(nullable: true),
                    SampleId = table.Column<int>(nullable: true),
                    SubjectId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characteristics", x => x.CharacteristicId);
                    table.ForeignKey(
                        name: "FK_Characteristics_CVterms_CVtermId",
                        column: x => x.CVtermId,
                        principalTable: "CVterms",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characteristics_CharacteristicObjects_CharacteristicFeatureId",
                        column: x => x.CharacteristicFeatureId,
                        principalTable: "CharacteristicObjects",
                        principalColumn: "CharacteristicObjId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characteristic_DataFile",
                        column: x => x.DatafileId,
                        principalTable: "DataFiles",
                        principalColumn: "DataFileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characteristic_Dataset",
                        column: x => x.DatasetId,
                        principalTable: "Datasets",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characteristics_BioSamples_SampleId",
                        column: x => x.SampleId,
                        principalTable: "BioSamples",
                        principalColumn: "BioSampleDBId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characteristics_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "SubjectDBId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_UserId",
                table: "UserAccounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccountClaims_UserAccountId",
                table: "UserAccountClaims",
                column: "UserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ProjectId",
                table: "Activities",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_DesignTypeId",
                table: "Activities",
                column: "DesignTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_MeasurementTypeId",
                table: "Activities",
                column: "MeasurementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_TechnologyPlatformId",
                table: "Activities",
                column: "TechnologyPlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_TechnologyTypeId",
                table: "Activities",
                column: "TechnologyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BioSamples_AssayId",
                table: "BioSamples",
                column: "AssayId");

            migrationBuilder.CreateIndex(
                name: "IX_BioSamples_CollectionStudyDayId",
                table: "BioSamples",
                column: "CollectionStudyDayId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BioSamples_CollectionStudyTimePointId",
                table: "BioSamples",
                column: "CollectionStudyTimePointId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BioSamples_DataFileId",
                table: "BioSamples",
                column: "DataFileId");

            migrationBuilder.CreateIndex(
                name: "IX_BioSamples_DatasetId",
                table: "BioSamples",
                column: "DatasetId");

            migrationBuilder.CreateIndex(
                name: "IX_BioSamples_StudyId",
                table: "BioSamples",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_BioSamples_SubjectId",
                table: "BioSamples",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_BioSamples_VisitId",
                table: "BioSamples",
                column: "VisitId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characteristics_CVtermId",
                table: "Characteristics",
                column: "CVtermId");

            migrationBuilder.CreateIndex(
                name: "IX_Characteristics_CharacteristicFeatureId",
                table: "Characteristics",
                column: "CharacteristicFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Characteristics_DatafileId",
                table: "Characteristics",
                column: "DatafileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characteristics_DatasetId",
                table: "Characteristics",
                column: "DatasetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characteristics_SampleId",
                table: "Characteristics",
                column: "SampleId");

            migrationBuilder.CreateIndex(
                name: "IX_Characteristics_SubjectId",
                table: "Characteristics",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicObjects_ActivityId",
                table: "CharacteristicObjects",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicObjects_CVtermId",
                table: "CharacteristicObjects",
                column: "CVtermId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicObjects_ProjectId",
                table: "CharacteristicObjects",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CVterms_DictionaryId",
                table: "CVterms",
                column: "DictionaryId");

            migrationBuilder.CreateIndex(
                name: "IX_CVterms_XrefId",
                table: "CVterms",
                column: "XrefId");

            migrationBuilder.CreateIndex(
                name: "IX_DBxreferences_DBId",
                table: "DBxreferences",
                column: "DBId");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_XrefId",
                table: "Dictionaries",
                column: "XrefId");

            migrationBuilder.CreateIndex(
                name: "IX_DataFiles_ProjectId",
                table: "DataFiles",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Datasets_ActivityId",
                table: "Datasets",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Datasets_TemplateId",
                table: "Datasets",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_VariableDefinitions_CVTermReferenceSourceId",
                table: "VariableDefinitions",
                column: "CVTermReferenceSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_VariableDefinitions_CVtermDictionaryId",
                table: "VariableDefinitions",
                column: "CVtermDictionaryId");

            migrationBuilder.CreateIndex(
                name: "IX_VariableDefinitions_ProjectId",
                table: "VariableDefinitions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_VariableDefinitions_RoleId",
                table: "VariableDefinitions",
                column: "RoleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VariableDefinitions_VariableTypeId",
                table: "VariableDefinitions",
                column: "VariableTypeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VariableReferences_ActivityDatasetId",
                table: "VariableReferences",
                column: "ActivityDatasetId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_StudyDayId",
                table: "Visits",
                column: "StudyDayId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visits_StudyId",
                table: "Visits",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_DatafileId",
                table: "Subjects",
                column: "DatafileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_DatasetId",
                table: "Subjects",
                column: "DatasetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_StudyArmId",
                table: "Subjects",
                column: "StudyArmId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_StudyId",
                table: "Subjects",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_ControlledTermId",
                table: "Observations",
                column: "ControlledTermId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Observations_DatafileId",
                table: "Observations",
                column: "DatafileId");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_DatasetId",
                table: "Observations",
                column: "DatasetId");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_DefaultQualifierId",
                table: "Observations",
                column: "DefaultQualifierId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Observations_ProjectId",
                table: "Observations",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_TopicVariableId",
                table: "Observations",
                column: "TopicVariableId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwnerId",
                table: "Projects",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Studies_ProjectId",
                table: "Studies",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainTemplateVariables_CVTermsDictionaryId",
                table: "DomainTemplateVariables",
                column: "CVTermsDictionaryId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainTemplateVariables_QualifiersDictionaryId",
                table: "DomainTemplateVariables",
                column: "QualifiersDictionaryId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainTemplateVariables_RoleTermId",
                table: "DomainTemplateVariables",
                column: "RoleTermId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainTemplateVariables_TemplateId",
                table: "DomainTemplateVariables",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainTemplateVariables_UsageTermId",
                table: "DomainTemplateVariables",
                column: "UsageTermId");

            migrationBuilder.CreateIndex(
                name: "IX_TimePoints_ReferenceTimePointId",
                table: "TimePoints",
                column: "ReferenceTimePointId");

            migrationBuilder.CreateIndex(
                name: "IX_Dataset_DataFiles_DatafileId",
                table: "Dataset_DataFiles",
                column: "DatafileId");

            migrationBuilder.CreateIndex(
                name: "IX_Observation_Qualfiers_QualifierId",
                table: "Observation_Qualfiers",
                column: "QualifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Observation_Synonyms_QualifierId",
                table: "Observation_Synonyms",
                column: "QualifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Observation_Timings_QualifierId",
                table: "Observation_Timings",
                column: "QualifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Users_UserId",
                table: "Project_Users",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Study_Arms_StudyId",
                table: "Study_Arms",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_Study_Datasets_StudyId",
                table: "Study_Datasets",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateField_TermSource_TermSourceId",
                table: "TemplateField_TermSource",
                column: "TermSourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAccountClaims");

            migrationBuilder.DropTable(
                name: "Characteristics");

            migrationBuilder.DropTable(
                name: "VariableReferences");

            migrationBuilder.DropTable(
                name: "Dataset_DataFiles");

            migrationBuilder.DropTable(
                name: "Observation_Qualfiers");

            migrationBuilder.DropTable(
                name: "Observation_Synonyms");

            migrationBuilder.DropTable(
                name: "Observation_Timings");

            migrationBuilder.DropTable(
                name: "Project_Users");

            migrationBuilder.DropTable(
                name: "Study_Arms");

            migrationBuilder.DropTable(
                name: "Study_Datasets");

            migrationBuilder.DropTable(
                name: "TemplateField_TermSource");

            migrationBuilder.DropTable(
                name: "UserAccounts");

            migrationBuilder.DropTable(
                name: "CharacteristicObjects");

            migrationBuilder.DropTable(
                name: "BioSamples");

            migrationBuilder.DropTable(
                name: "Observations");

            migrationBuilder.DropTable(
                name: "DomainTemplateVariables");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Visits");

            migrationBuilder.DropTable(
                name: "VariableDefinitions");

            migrationBuilder.DropTable(
                name: "DataFiles");

            migrationBuilder.DropTable(
                name: "Datasets");

            migrationBuilder.DropTable(
                name: "Arms");

            migrationBuilder.DropTable(
                name: "TimePoints");

            migrationBuilder.DropTable(
                name: "Studies");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "DomainTemplates");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "CVterms");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Dictionaries");

            migrationBuilder.DropTable(
                name: "DBxreferences");

            migrationBuilder.DropTable(
                name: "Dbs");
        }
    }
}
