namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSampleSubjectVisit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Visit_TBL",
                c => new
                    {
                        VisitId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2000, storeType: "nvarchar"),
                        Number = c.Int(),
                        StudyDayId = c.Int(nullable: false),
                        StudyId = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.VisitId)
                .ForeignKey("Study_TBL", t => t.StudyId, cascadeDelete: true)
                .ForeignKey("TimePoints", t => t.StudyDayId, cascadeDelete: true)
                .Index(t => t.StudyDayId)
                .Index(t => t.StudyId);
            
            CreateTable(
                "TimePoints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Number = c.Int(),
                        ReferenceTimePointId = c.Int(),
                        DateTime = c.DateTime(precision: 0),
                        Discriminator = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TimePoints", t => t.ReferenceTimePointId)
                .Index(t => t.ReferenceTimePointId);
            
            CreateTable(
                "BioSamples_TBL",
                c => new
                    {
                        BioSampleDBId = c.Int(nullable: false, identity: true),
                        SubjectId = c.String(unicode: false),
                        BiosampleStudyId = c.String(unicode: false),
                        StudyId = c.String(maxLength: 200, storeType: "nvarchar"),
                        AssayId = c.Int(nullable: false),
                        DatasetId = c.Int(nullable: false),
                        VisitId = c.Int(nullable: false),
                        CollectionDateTime_Id = c.Int(),
                        CollectionStudyDay_Id = c.Int(),
                        CollectionStudyTimePoint_Id = c.Int(),
                    })
                .PrimaryKey(t => t.BioSampleDBId)
                .ForeignKey("Activity_TBL", t => t.AssayId, cascadeDelete: true)
                .ForeignKey("TimePoints", t => t.CollectionDateTime_Id)
                .ForeignKey("TimePoints", t => t.CollectionStudyDay_Id)
                .ForeignKey("TimePoints", t => t.CollectionStudyTimePoint_Id)
                .ForeignKey("Dataset_TBL", t => t.DatasetId, cascadeDelete: true)
                .ForeignKey("Study_TBL", t => t.StudyId)
                .ForeignKey("Visit_TBL", t => t.VisitId, cascadeDelete: true)
                .Index(t => t.StudyId)
                .Index(t => t.AssayId)
                .Index(t => t.DatasetId)
                .Index(t => t.VisitId)
                .Index(t => t.CollectionDateTime_Id)
                .Index(t => t.CollectionStudyDay_Id)
                .Index(t => t.CollectionStudyTimePoint_Id);
            
            CreateTable(
                "Characteristics_TBL",
                c => new
                    {
                        CharacterisitcId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2000, storeType: "nvarchar"),
                        Value = c.String(unicode: false),
                        CVtermId = c.String(maxLength: 200, storeType: "nvarchar"),
                        DatasetDomainCode = c.String(unicode: false),
                        SampleId = c.Int(),
                        SubjectId = c.String(maxLength: 128, storeType: "nvarchar"),
                        Discriminator = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        DatasetVariableId = c.Int(nullable: false),
                        DatasetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CharacterisitcId)
                .ForeignKey("BioSamples_TBL", t => t.SampleId, cascadeDelete: true)
                .ForeignKey("Subject_TBL", t => t.SubjectId)
                .ForeignKey("CVterm_TBL", t => t.CVtermId)
                .ForeignKey("Variable_Reference_TBL", t => new { t.DatasetVariableId, t.DatasetId })
                .Index(t => t.CVtermId)
                .Index(t => t.SampleId)
                .Index(t => t.SubjectId)
                .Index(t => new { t.DatasetVariableId, t.DatasetId });
            
            CreateTable(
                "Subject_TBL",
                c => new
                    {
                        SubjectDBId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        StudyId = c.String(maxLength: 200, storeType: "nvarchar"),
                        SubjectStudyId = c.String(unicode: false),
                        UniqueSubjectId = c.String(unicode: false),
                        SubjectStartDate = c.DateTime(nullable: false, precision: 0),
                        SubjectEndDate = c.DateTime(nullable: false, precision: 0),
                        Arm = c.String(unicode: false),
                        ArmCode = c.String(unicode: false),
                        DatasetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SubjectDBId)
                .ForeignKey("Dataset_TBL", t => t.DatasetId, cascadeDelete: true)
                .ForeignKey("Study_TBL", t => t.StudyId)
                .Index(t => t.StudyId)
                .Index(t => t.DatasetId);
            
            CreateTable(
                "Visit_TimePoints",
                c => new
                    {
                        VisitId = c.Int(nullable: false),
                        TimePointId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.VisitId, t.TimePointId })
                .ForeignKey("Visit_TBL", t => t.VisitId, cascadeDelete: true)
                .ForeignKey("TimePoints", t => t.TimePointId, cascadeDelete: true)
                .Index(t => t.VisitId)
                .Index(t => t.TimePointId);
            
            AddColumn("Study_TBL", "Site", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropForeignKey("Characteristics_TBL", new[] { "DatasetVariableId", "DatasetId" }, "Variable_Reference_TBL");
            DropForeignKey("Characteristics_TBL", "CVtermId", "CVterm_TBL");
            DropForeignKey("Characteristics_TBL", "SubjectId", "Subject_TBL");
            DropForeignKey("Subject_TBL", "StudyId", "Study_TBL");
            DropForeignKey("Subject_TBL", "DatasetId", "Dataset_TBL");
            DropForeignKey("BioSamples_TBL", "VisitId", "Visit_TBL");
            DropForeignKey("BioSamples_TBL", "StudyId", "Study_TBL");
            DropForeignKey("Characteristics_TBL", "SampleId", "BioSamples_TBL");
            DropForeignKey("BioSamples_TBL", "DatasetId", "Dataset_TBL");
            DropForeignKey("BioSamples_TBL", "CollectionStudyTimePoint_Id", "TimePoints");
            DropForeignKey("BioSamples_TBL", "CollectionStudyDay_Id", "TimePoints");
            DropForeignKey("BioSamples_TBL", "CollectionDateTime_Id", "TimePoints");
            DropForeignKey("BioSamples_TBL", "AssayId", "Activity_TBL");
            DropForeignKey("Visit_TimePoints", "TimePointId", "TimePoints");
            DropForeignKey("Visit_TimePoints", "VisitId", "Visit_TBL");
            DropForeignKey("Visit_TBL", "StudyDayId", "TimePoints");
            DropForeignKey("TimePoints", "ReferenceTimePointId", "TimePoints");
            DropForeignKey("Visit_TBL", "StudyId", "Study_TBL");
            DropIndex("Visit_TimePoints", new[] { "TimePointId" });
            DropIndex("Visit_TimePoints", new[] { "VisitId" });
            DropIndex("Subject_TBL", new[] { "DatasetId" });
            DropIndex("Subject_TBL", new[] { "StudyId" });
            DropIndex("Characteristics_TBL", new[] { "DatasetVariableId", "DatasetId" });
            DropIndex("Characteristics_TBL", new[] { "SubjectId" });
            DropIndex("Characteristics_TBL", new[] { "SampleId" });
            DropIndex("Characteristics_TBL", new[] { "CVtermId" });
            DropIndex("BioSamples_TBL", new[] { "CollectionStudyTimePoint_Id" });
            DropIndex("BioSamples_TBL", new[] { "CollectionStudyDay_Id" });
            DropIndex("BioSamples_TBL", new[] { "CollectionDateTime_Id" });
            DropIndex("BioSamples_TBL", new[] { "VisitId" });
            DropIndex("BioSamples_TBL", new[] { "DatasetId" });
            DropIndex("BioSamples_TBL", new[] { "AssayId" });
            DropIndex("BioSamples_TBL", new[] { "StudyId" });
            DropIndex("TimePoints", new[] { "ReferenceTimePointId" });
            DropIndex("Visit_TBL", new[] { "StudyId" });
            DropIndex("Visit_TBL", new[] { "StudyDayId" });
            DropColumn("Study_TBL", "Site");
            DropTable("Visit_TimePoints");
            DropTable("Subject_TBL");
            DropTable("Characteristics_TBL");
            DropTable("BioSamples_TBL");
            DropTable("TimePoints");
            DropTable("Visit_TBL");
        }
    }
}
