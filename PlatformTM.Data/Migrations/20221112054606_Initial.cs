using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformTM.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cohorts",
                columns: table => new
                {
                    CohortId = table.Column<string>(type: "varchar(95)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CohortCode = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CohortName = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cohorts", x => x.CohortId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Dbs",
                columns: table => new
                {
                    OID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Version = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    URL = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    URLPrefix = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dbs", x => x.OID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DomainTemplates",
                columns: table => new
                {
                    OID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Domain = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Class = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Structure = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRepeating = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainTemplates", x => x.OID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TimePoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Number = table.Column<int>(type: "int", nullable: true),
                    Discriminator = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    ReferenceTimePointId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimePoints_TimePoints_ReferenceTimePointId",
                        column: x => x.ReferenceTimePointId,
                        principalTable: "TimePoints",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FirstName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Organization = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DBxreferences",
                columns: table => new
                {
                    OID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Accession = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DBId = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Accession = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Title = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsPublic = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    OwnerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserAccounts",
                columns: table => new
                {
                    UserAccountId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AdminApproved = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    PSK = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Dictionaries",
                columns: table => new
                {
                    OID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Definition = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    XrefId = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictionaries", x => x.OID);
                    table.ForeignKey(
                        name: "FK_Dictionaries_DBxreferences_XrefId",
                        column: x => x.XrefId,
                        principalTable: "DBxreferences",
                        principalColumn: "OID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PrimaryDataset",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Domain = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    DescriptorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Version = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrimaryDataset", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrimaryDataset_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProjectUser",
                columns: table => new
                {
                    AffiliatedProjectsId = table.Column<int>(type: "int", nullable: false),
                    MembersId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUser", x => new { x.AffiliatedProjectsId, x.MembersId });
                    table.ForeignKey(
                        name: "FK_ProjectUser_Projects_AffiliatedProjectsId",
                        column: x => x.AffiliatedProjectsId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectUser_Users_MembersId",
                        column: x => x.MembersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Studies",
                columns: table => new
                {
                    StudyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Accession = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Site = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProjectId = table.Column<int>(type: "int", nullable: false)
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserAccountClaims",
                columns: table => new
                {
                    CalimId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserAccountId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CVterms",
                columns: table => new
                {
                    OID = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Definition = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsUserSpecified = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    Synonyms = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    XrefId = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DictionaryId = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CVterms", x => x.OID);
                    table.ForeignKey(
                        name: "FK_CVterms_DBxreferences_XrefId",
                        column: x => x.XrefId,
                        principalTable: "DBxreferences",
                        principalColumn: "OID");
                    table.ForeignKey(
                        name: "FK_CVterms_Dictionaries_DictionaryId",
                        column: x => x.DictionaryId,
                        principalTable: "Dictionaries",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CohortStudy",
                columns: table => new
                {
                    CohortsId = table.Column<string>(type: "varchar(95)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StudiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CohortStudy", x => new { x.CohortsId, x.StudiesId });
                    table.ForeignKey(
                        name: "FK_CohortStudy_Cohorts_CohortsId",
                        column: x => x.CohortsId,
                        principalTable: "Cohorts",
                        principalColumn: "CohortId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CohortStudy_Studies_StudiesId",
                        column: x => x.StudiesId,
                        principalTable: "Studies",
                        principalColumn: "StudyId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PrimaryDatasetStudy",
                columns: table => new
                {
                    PrimaryDatasetsId = table.Column<int>(type: "int", nullable: false),
                    StudiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrimaryDatasetStudy", x => new { x.PrimaryDatasetsId, x.StudiesId });
                    table.ForeignKey(
                        name: "FK_PrimaryDatasetStudy_PrimaryDataset_PrimaryDatasetsId",
                        column: x => x.PrimaryDatasetsId,
                        principalTable: "PrimaryDataset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrimaryDatasetStudy_Studies_StudiesId",
                        column: x => x.StudiesId,
                        principalTable: "Studies",
                        principalColumn: "StudyId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Visits",
                columns: table => new
                {
                    VisitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Number = table.Column<int>(type: "int", nullable: true),
                    StudyDayId = table.Column<int>(type: "int", nullable: false),
                    StudyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visits", x => x.VisitId);
                    table.ForeignKey(
                        name: "FK_Visits_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "StudyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Visits_TimePoints_StudyDayId",
                        column: x => x.StudyDayId,
                        principalTable: "TimePoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CharacteristicObjects",
                columns: table => new
                {
                    CharacteristicObjId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ShortName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FullName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Domain = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CVtermId = table.Column<string>(type: "varchar(127)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ActivityId = table.Column<int>(type: "int", nullable: true),
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacteristicObjects", x => x.CharacteristicObjId);
                    table.ForeignKey(
                        name: "FK_CharacteristicObjects_CVterms_CVtermId",
                        column: x => x.CVtermId,
                        principalTable: "CVterms",
                        principalColumn: "OID");
                    table.ForeignKey(
                        name: "FK_CharacteristicObjects_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DomainTemplateVariables",
                columns: table => new
                {
                    OID = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataType = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Label = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Section = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AllowMultipleValues = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsGeneric = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    QualifiersDictionaryId = table.Column<string>(type: "varchar(10)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TemplateId = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleTermId = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsageTermId = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CVTermsDictionaryId = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainTemplateVariables", x => x.OID);
                    table.ForeignKey(
                        name: "FK_DomainTemplateVariables_CVterms_RoleTermId",
                        column: x => x.RoleTermId,
                        principalTable: "CVterms",
                        principalColumn: "OID");
                    table.ForeignKey(
                        name: "FK_DomainTemplateVariables_CVterms_UsageTermId",
                        column: x => x.UsageTermId,
                        principalTable: "CVterms",
                        principalColumn: "OID");
                    table.ForeignKey(
                        name: "FK_DomainTemplateVariables_Dictionaries_CVTermsDictionaryId",
                        column: x => x.CVTermsDictionaryId,
                        principalTable: "Dictionaries",
                        principalColumn: "OID");
                    table.ForeignKey(
                        name: "FK_DomainTemplateVariables_Dictionaries_QualifiersDictionaryId",
                        column: x => x.QualifiersDictionaryId,
                        principalTable: "Dictionaries",
                        principalColumn: "OID");
                    table.ForeignKey(
                        name: "FK_DomainTemplateVariables_DomainTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "DomainTemplates",
                        principalColumn: "OID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Assessment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StudyId = table.Column<int>(type: "int", nullable: false),
                    TimeEventId = table.Column<int>(type: "int", nullable: true),
                    Discriminator = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DesignTypeId = table.Column<string>(type: "varchar(127)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TechnologyTypeId = table.Column<string>(type: "varchar(127)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TechnologyPlatformId = table.Column<string>(type: "varchar(127)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MeasurementTypeId = table.Column<string>(type: "varchar(127)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assessment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assessment_CVterms_DesignTypeId",
                        column: x => x.DesignTypeId,
                        principalTable: "CVterms",
                        principalColumn: "OID");
                    table.ForeignKey(
                        name: "FK_Assessment_CVterms_MeasurementTypeId",
                        column: x => x.MeasurementTypeId,
                        principalTable: "CVterms",
                        principalColumn: "OID");
                    table.ForeignKey(
                        name: "FK_Assessment_CVterms_TechnologyPlatformId",
                        column: x => x.TechnologyPlatformId,
                        principalTable: "CVterms",
                        principalColumn: "OID");
                    table.ForeignKey(
                        name: "FK_Assessment_CVterms_TechnologyTypeId",
                        column: x => x.TechnologyTypeId,
                        principalTable: "CVterms",
                        principalColumn: "OID");
                    table.ForeignKey(
                        name: "FK_Assessment_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "StudyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assessment_Visits_TimeEventId",
                        column: x => x.TimeEventId,
                        principalTable: "Visits",
                        principalColumn: "VisitId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TemplateField_TermSource",
                columns: table => new
                {
                    TemplateFieldId = table.Column<string>(type: "varchar(10)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TermSourceId = table.Column<string>(type: "varchar(10)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AssessmentPrimaryDataset",
                columns: table => new
                {
                    AssessmentsId = table.Column<int>(type: "int", nullable: false),
                    DatasetsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentPrimaryDataset", x => new { x.AssessmentsId, x.DatasetsId });
                    table.ForeignKey(
                        name: "FK_AssessmentPrimaryDataset_Assessment_AssessmentsId",
                        column: x => x.AssessmentsId,
                        principalTable: "Assessment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssessmentPrimaryDataset_PrimaryDataset_DatasetsId",
                        column: x => x.DatasetsId,
                        principalTable: "PrimaryDataset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DataFiles",
                columns: table => new
                {
                    DataFileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FileName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Format = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Size = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    State = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Path = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    DatasetId = table.Column<int>(type: "int", nullable: true),
                    AssessmentId = table.Column<int>(type: "int", nullable: true),
                    IsDirectory = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsLoadedToDB = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    Version = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataFiles", x => x.DataFileId);
                    table.ForeignKey(
                        name: "FK_DataFiles_Assessment_AssessmentId",
                        column: x => x.AssessmentId,
                        principalTable: "Assessment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DataFiles_DataFiles_ParentId",
                        column: x => x.ParentId,
                        principalTable: "DataFiles",
                        principalColumn: "DataFileId");
                    table.ForeignKey(
                        name: "FK_DataFiles_PrimaryDataset_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "PrimaryDataset",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DataFiles_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    SubjectDBId = table.Column<string>(type: "varchar(95)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SubjectStudyId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UniqueSubjectId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StudyId = table.Column<int>(type: "int", nullable: false),
                    SubjectStartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    SubjectEndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    StudyCohortId = table.Column<string>(type: "varchar(95)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatasetId = table.Column<int>(type: "int", nullable: false),
                    SourceDatafileId = table.Column<int>(type: "int", nullable: false),
                    CohortId = table.Column<string>(type: "varchar(95)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.SubjectDBId);
                    table.ForeignKey(
                        name: "FK_Subjects_Cohorts_CohortId",
                        column: x => x.CohortId,
                        principalTable: "Cohorts",
                        principalColumn: "CohortId");
                    table.ForeignKey(
                        name: "FK_Subjects_Cohorts_StudyCohortId",
                        column: x => x.StudyCohortId,
                        principalTable: "Cohorts",
                        principalColumn: "CohortId");
                    table.ForeignKey(
                        name: "FK_Subjects_DataFiles_SourceDatafileId",
                        column: x => x.SourceDatafileId,
                        principalTable: "DataFiles",
                        principalColumn: "DataFileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subjects_PrimaryDataset_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "PrimaryDataset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subjects_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "StudyId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BioSamples",
                columns: table => new
                {
                    BioSampleDBId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BiosampleStudyId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsBaseline = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    SubjectId = table.Column<string>(type: "varchar(95)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StudyId = table.Column<int>(type: "int", nullable: false),
                    AssayId = table.Column<int>(type: "int", nullable: false),
                    DatasetId = table.Column<int>(type: "int", nullable: false),
                    DataFileId = table.Column<int>(type: "int", nullable: true),
                    VisitId = table.Column<int>(type: "int", nullable: true),
                    CollectionDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    CollectionStudyDayId = table.Column<int>(type: "int", nullable: true),
                    CollectionStudyTimePointId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BioSamples", x => x.BioSampleDBId);
                    table.ForeignKey(
                        name: "FK_BioSamples_Assessment_AssayId",
                        column: x => x.AssayId,
                        principalTable: "Assessment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BioSamples_DataFiles_DataFileId",
                        column: x => x.DataFileId,
                        principalTable: "DataFiles",
                        principalColumn: "DataFileId");
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
                        principalColumn: "SubjectDBId");
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
                        name: "FK_BioSamples_Visits_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visits",
                        principalColumn: "VisitId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Characteristics",
                columns: table => new
                {
                    CharacteristicId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacteristicFeatureId = table.Column<int>(type: "int", nullable: false),
                    VerbatimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ControlledValueStr = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CharacObjName = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CVtermId = table.Column<string>(type: "varchar(127)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatafileId = table.Column<int>(type: "int", nullable: true),
                    DatasetId = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SampleId = table.Column<int>(type: "int", nullable: true),
                    SubjectId = table.Column<string>(type: "varchar(95)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characteristics", x => x.CharacteristicId);
                    table.ForeignKey(
                        name: "FK_Characteristic_DataFile",
                        column: x => x.DatafileId,
                        principalTable: "DataFiles",
                        principalColumn: "DataFileId");
                    table.ForeignKey(
                        name: "FK_Characteristics_BioSamples_SampleId",
                        column: x => x.SampleId,
                        principalTable: "BioSamples",
                        principalColumn: "BioSampleDBId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characteristics_CharacteristicObjects_CharacteristicFeatureId",
                        column: x => x.CharacteristicFeatureId,
                        principalTable: "CharacteristicObjects",
                        principalColumn: "CharacteristicObjId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characteristics_CVterms_CVtermId",
                        column: x => x.CVtermId,
                        principalTable: "CVterms",
                        principalColumn: "OID");
                    table.ForeignKey(
                        name: "FK_Characteristics_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "SubjectDBId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Assessment_DesignTypeId",
                table: "Assessment",
                column: "DesignTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessment_MeasurementTypeId",
                table: "Assessment",
                column: "MeasurementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessment_StudyId",
                table: "Assessment",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessment_TechnologyPlatformId",
                table: "Assessment",
                column: "TechnologyPlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessment_TechnologyTypeId",
                table: "Assessment",
                column: "TechnologyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessment_TimeEventId",
                table: "Assessment",
                column: "TimeEventId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentPrimaryDataset_DatasetsId",
                table: "AssessmentPrimaryDataset",
                column: "DatasetsId");

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
                name: "IX_CharacteristicObjects_CVtermId",
                table: "CharacteristicObjects",
                column: "CVtermId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicObjects_ProjectId",
                table: "CharacteristicObjects",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Characteristics_CharacteristicFeatureId",
                table: "Characteristics",
                column: "CharacteristicFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Characteristics_CVtermId",
                table: "Characteristics",
                column: "CVtermId");

            migrationBuilder.CreateIndex(
                name: "IX_Characteristics_DatafileId",
                table: "Characteristics",
                column: "DatafileId");

            migrationBuilder.CreateIndex(
                name: "IX_Characteristics_SampleId",
                table: "Characteristics",
                column: "SampleId");

            migrationBuilder.CreateIndex(
                name: "IX_Characteristics_SubjectId",
                table: "Characteristics",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CohortStudy_StudiesId",
                table: "CohortStudy",
                column: "StudiesId");

            migrationBuilder.CreateIndex(
                name: "IX_CVterms_DictionaryId",
                table: "CVterms",
                column: "DictionaryId");

            migrationBuilder.CreateIndex(
                name: "IX_CVterms_XrefId",
                table: "CVterms",
                column: "XrefId");

            migrationBuilder.CreateIndex(
                name: "IX_DataFiles_AssessmentId",
                table: "DataFiles",
                column: "AssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DataFiles_DatasetId",
                table: "DataFiles",
                column: "DatasetId");

            migrationBuilder.CreateIndex(
                name: "IX_DataFiles_ParentId",
                table: "DataFiles",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_DataFiles_ProjectId",
                table: "DataFiles",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DBxreferences_DBId",
                table: "DBxreferences",
                column: "DBId");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_XrefId",
                table: "Dictionaries",
                column: "XrefId");

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
                name: "IX_PrimaryDataset_ProjectId",
                table: "PrimaryDataset",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PrimaryDatasetStudy_StudiesId",
                table: "PrimaryDatasetStudy",
                column: "StudiesId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwnerId",
                table: "Projects",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUser_MembersId",
                table: "ProjectUser",
                column: "MembersId");

            migrationBuilder.CreateIndex(
                name: "IX_Studies_ProjectId",
                table: "Studies",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_CohortId",
                table: "Subjects",
                column: "CohortId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_DatasetId",
                table: "Subjects",
                column: "DatasetId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_SourceDatafileId",
                table: "Subjects",
                column: "SourceDatafileId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_StudyCohortId",
                table: "Subjects",
                column: "StudyCohortId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_StudyId",
                table: "Subjects",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateField_TermSource_TermSourceId",
                table: "TemplateField_TermSource",
                column: "TermSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_TimePoints_ReferenceTimePointId",
                table: "TimePoints",
                column: "ReferenceTimePointId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccountClaims_UserAccountId",
                table: "UserAccountClaims",
                column: "UserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_UserId",
                table: "UserAccounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_StudyDayId",
                table: "Visits",
                column: "StudyDayId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visits_StudyId",
                table: "Visits",
                column: "StudyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssessmentPrimaryDataset");

            migrationBuilder.DropTable(
                name: "Characteristics");

            migrationBuilder.DropTable(
                name: "CohortStudy");

            migrationBuilder.DropTable(
                name: "PrimaryDatasetStudy");

            migrationBuilder.DropTable(
                name: "ProjectUser");

            migrationBuilder.DropTable(
                name: "TemplateField_TermSource");

            migrationBuilder.DropTable(
                name: "UserAccountClaims");

            migrationBuilder.DropTable(
                name: "BioSamples");

            migrationBuilder.DropTable(
                name: "CharacteristicObjects");

            migrationBuilder.DropTable(
                name: "DomainTemplateVariables");

            migrationBuilder.DropTable(
                name: "UserAccounts");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "DomainTemplates");

            migrationBuilder.DropTable(
                name: "Cohorts");

            migrationBuilder.DropTable(
                name: "DataFiles");

            migrationBuilder.DropTable(
                name: "Assessment");

            migrationBuilder.DropTable(
                name: "PrimaryDataset");

            migrationBuilder.DropTable(
                name: "CVterms");

            migrationBuilder.DropTable(
                name: "Visits");

            migrationBuilder.DropTable(
                name: "Dictionaries");

            migrationBuilder.DropTable(
                name: "Studies");

            migrationBuilder.DropTable(
                name: "TimePoints");

            migrationBuilder.DropTable(
                name: "DBxreferences");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Dbs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
