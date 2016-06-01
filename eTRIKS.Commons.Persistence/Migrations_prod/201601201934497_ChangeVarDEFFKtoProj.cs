namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeVarDEFFKtoProj : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Variable_Definition_TBL", "StudyId", "Study_TBL");
            DropIndex("Variable_Definition_TBL", new[] { "StudyId" });
            AddColumn("Variable_Definition_TBL", "ProjectId", c => c.Int(nullable: false));
            CreateIndex("Variable_Definition_TBL", "ProjectId");
            AddForeignKey("Variable_Definition_TBL", "ProjectId", "Project_TBL", "ProjectId", cascadeDelete: true);
            DropColumn("Variable_Definition_TBL", "StudyId");
        }
        
        public override void Down()
        {
            AddColumn("Variable_Definition_TBL", "StudyId", c => c.Int(nullable: false));
            DropForeignKey("Variable_Definition_TBL", "ProjectId", "Project_TBL");
            DropIndex("Variable_Definition_TBL", new[] { "ProjectId" });
            DropColumn("Variable_Definition_TBL", "ProjectId");
            CreateIndex("Variable_Definition_TBL", "StudyId");
            AddForeignKey("Variable_Definition_TBL", "StudyId", "Study_TBL", "StudyId", cascadeDelete: true);
        }
    }
}
