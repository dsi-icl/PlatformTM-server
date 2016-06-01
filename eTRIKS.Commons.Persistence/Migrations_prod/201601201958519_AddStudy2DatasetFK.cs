namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStudy2DatasetFK : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Study_Datasets",
                c => new
                    {
                        StudyId = c.Int(nullable: false),
                        DatasetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.StudyId, t.DatasetId })
                .ForeignKey("Study_TBL", t => t.StudyId, cascadeDelete: true)
                .ForeignKey("Dataset_TBL", t => t.DatasetId, cascadeDelete: true)
                .Index(t => t.StudyId)
                .Index(t => t.DatasetId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Study_Datasets", "DatasetId", "Dataset_TBL");
            DropForeignKey("Study_Datasets", "StudyId", "Study_TBL");
            DropIndex("Study_Datasets", new[] { "DatasetId" });
            DropIndex("Study_Datasets", new[] { "StudyId" });
            DropTable("Study_Datasets");
        }
    }
}
