namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAct2ProjInsteadOfStudy : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Activity_TBL", "StudyId", "Study_TBL");
            DropIndex("Activity_TBL", new[] { "StudyId" });
            AddColumn("Activity_TBL", "ProjectId", c => c.Int(nullable: false));
            CreateIndex("Activity_TBL", "ProjectId");
            AddForeignKey("Activity_TBL", "ProjectId", "Project_TBL", "ProjectId", cascadeDelete: true);
            DropColumn("Activity_TBL", "StudyId");
        }
        
        public override void Down()
        {
            AddColumn("Activity_TBL", "StudyId", c => c.Int(nullable: false));
            DropForeignKey("Activity_TBL", "ProjectId", "Project_TBL");
            DropIndex("Activity_TBL", new[] { "ProjectId" });
            DropColumn("Activity_TBL", "ProjectId");
            CreateIndex("Activity_TBL", "StudyId");
            AddForeignKey("Activity_TBL", "StudyId", "Study_TBL", "StudyId", cascadeDelete: true);
        }
    }
}
