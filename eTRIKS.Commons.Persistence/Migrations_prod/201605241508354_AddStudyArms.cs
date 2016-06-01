namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStudyArms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Arm_TBL",
                c => new
                    {
                        ArmId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        ArmCode = c.String(nullable: false, unicode: false),
                        ArmName = c.String(nullable: false, maxLength: 2000, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.ArmId);
            
            CreateTable(
                "Study_Arms",
                c => new
                    {
                        ArmId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        StudyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ArmId, t.StudyId })
                .ForeignKey("ARMS", t => t.ArmId, cascadeDelete: true)
                .ForeignKey("Study_TBL", t => t.StudyId, cascadeDelete: true)
                .Index(t => t.ArmId)
                .Index(t => t.StudyId);
            
            AddColumn("Subject_TBL", "StudyArm_Id", c => c.String(maxLength: 128, storeType: "nvarchar"));
            CreateIndex("Subject_TBL", "StudyArm_Id");
            AddForeignKey("Subject_TBL", "StudyArm_Id", "ARMS", "ArmId");
        }
        
        public override void Down()
        {
            DropForeignKey("Subject_TBL", "StudyArm_Id", "ARMS");
            DropForeignKey("Study_Arms", "StudyId", "Study_TBL");
            DropForeignKey("Study_Arms", "ArmId", "ARMS");
            DropIndex("Study_Arms", new[] { "StudyId" });
            DropIndex("Study_Arms", new[] { "ArmId" });
            DropIndex("Subject_TBL", new[] { "StudyArm_Id" });
            DropColumn("Subject_TBL", "StudyArm_Id");
            DropTable("Study_Arms");
            DropTable("ARMS");
        }
    }
}
