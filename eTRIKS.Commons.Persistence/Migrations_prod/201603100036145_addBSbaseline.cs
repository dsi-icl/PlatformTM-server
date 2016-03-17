namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addBSbaseline : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.BioSamples_TBL", "VisitId", "dbo.Visit_TBL");
            //DropIndex("dbo.BioSamples_TBL", new[] { "VisitId" });
            AddColumn("dbo.BioSamples_TBL", "IsBaseline", c => c.Boolean(nullable: false));
            //AlterColumn("dbo.BioSamples_TBL", "SubjectId", c => c.String(maxLength: 128, storeType: "nvarchar"));
            //AlterColumn("dbo.BioSamples_TBL", "VisitId", c => c.Int());
            //CreateIndex("dbo.BioSamples_TBL", "SubjectId");
            //CreateIndex("dbo.BioSamples_TBL", "VisitId");
            //AddForeignKey("dbo.BioSamples_TBL", "SubjectId", "dbo.Subject_TBL", "SubjectDBId");
            //AddForeignKey("dbo.BioSamples_TBL", "VisitId", "dbo.Visit_TBL", "VisitId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BioSamples_TBL", "VisitId", "dbo.Visit_TBL");
            DropForeignKey("dbo.BioSamples_TBL", "SubjectId", "dbo.Subject_TBL");
            DropIndex("dbo.BioSamples_TBL", new[] { "VisitId" });
            DropIndex("dbo.BioSamples_TBL", new[] { "SubjectId" });
            AlterColumn("dbo.BioSamples_TBL", "VisitId", c => c.Int(nullable: false));
            AlterColumn("dbo.BioSamples_TBL", "SubjectId", c => c.String(unicode: false));
            DropColumn("dbo.BioSamples_TBL", "IsBaseline");
            CreateIndex("dbo.BioSamples_TBL", "VisitId");
            AddForeignKey("dbo.BioSamples_TBL", "VisitId", "dbo.Visit_TBL", "VisitId", cascadeDelete: true);
        }
    }
}
