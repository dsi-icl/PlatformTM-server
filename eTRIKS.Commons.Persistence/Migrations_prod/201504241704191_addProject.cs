namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addProject : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Project_TBL",
                c => new
                    {
                        ProjectId = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Description = c.String(unicode: false),
                        ShortName = c.String(unicode: false),
                        Accession = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.ProjectId);
            
            CreateTable(
                "Study_Observations",
                c => new
                    {
                        StudyId = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        ObservationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.StudyId, t.ObservationId })
                .ForeignKey("Study_TBL", t => t.StudyId, cascadeDelete: true)
                .ForeignKey("Observation_TBL", t => t.ObservationId, cascadeDelete: true)
                .Index(t => t.StudyId)
                .Index(t => t.ObservationId);
            
            AddColumn("Study_TBL", "ProjectId", c => c.Int(nullable: true));
            CreateIndex("Study_TBL", "ProjectId");
            AddForeignKey("Study_TBL", "ProjectId", "Project_TBL", "ProjectId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("Study_TBL", "ProjectId", "Project_TBL");
            DropForeignKey("Study_Observations", "ObservationId", "Observation_TBL");
            DropForeignKey("Study_Observations", "StudyId", "Study_TBL");
            DropIndex("Study_Observations", new[] { "ObservationId" });
            DropIndex("Study_Observations", new[] { "StudyId" });
            DropIndex("Study_TBL", new[] { "ProjectId" });
            DropColumn("Study_TBL", "ProjectId");
            DropTable("Study_Observations");
            DropTable("Project_TBL");
        }
    }
}
