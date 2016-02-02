namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeStudyPKint : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("Variable_Definition_TBL", "StudyId", "Study_TBL");
            //DropForeignKey("BioSamples_TBL", "StudyId", "Study_TBL");
            //DropForeignKey("Subject_TBL", "StudyId", "Study_TBL");
            //DropForeignKey("Study_Observations", "StudyId", "Study_TBL");
            //DropForeignKey("Visit_TBL", "StudyId", "Study_TBL");
            //DropForeignKey("Activity_TBL", "StudyId", "Study_TBL");
            //DropIndex("Activity_TBL", new[] { "StudyId" });
            //DropIndex("Variable_Definition_TBL", new[] { "StudyId" });
            //DropIndex("Visit_TBL", new[] { "StudyId" });
            //DropIndex("BioSamples_TBL", new[] { "StudyId" });
            //DropIndex("Subject_TBL", new[] { "StudyId" });
            //DropIndex("Study_Observations", new[] { "StudyId" });
            //RenameColumn(table: "Study_TBL", name: "OID", newName: "Accession");
            //DropPrimaryKey("Study_TBL");
            //DropPrimaryKey("Study_Observations");
            ////AddColumn("Study_TBL", "Accession", c => c.String(unicode: false));
            //AddColumn("Characteristics_TBL", "ControlledTermStr", c => c.String(unicode: false));
            //AlterColumn("Activity_TBL", "StudyId", c => c.Int(nullable: false));
            //AlterColumn("Study_TBL", "StudyId", c => c.Int(nullable: false, identity: true));
            //AlterColumn("Variable_Definition_TBL", "StudyId", c => c.Int(nullable: false));
            //AlterColumn("Visit_TBL", "StudyId", c => c.Int(nullable: false));
            //AlterColumn("BioSamples_TBL", "StudyId", c => c.Int(nullable: false));
            //AlterColumn("Subject_TBL", "StudyId", c => c.Int(nullable: false));
            //AlterColumn("Study_Observations", "StudyId", c => c.Int(nullable: false));
            //AddPrimaryKey("Study_TBL", "StudyId");
            //AddPrimaryKey("Study_Observations", new[] { "StudyId", "ObservationId" });
            //CreateIndex("Activity_TBL", "StudyId");
            //CreateIndex("Variable_Definition_TBL", "StudyId");
            //CreateIndex("Visit_TBL", "StudyId");
            //CreateIndex("BioSamples_TBL", "StudyId");
            //CreateIndex("Subject_TBL", "StudyId");
            //CreateIndex("Study_Observations", "StudyId");
            //AddForeignKey("Variable_Definition_TBL", "StudyId", "Study_TBL", "StudyId", cascadeDelete: true);
            //AddForeignKey("BioSamples_TBL", "StudyId", "Study_TBL", "StudyId", cascadeDelete: true);
            //AddForeignKey("Subject_TBL", "StudyId", "Study_TBL", "StudyId", cascadeDelete: true);
            //AddForeignKey("Study_Observations", "StudyId", "Study_TBL", "StudyId", cascadeDelete: true);
            //AddForeignKey("Visit_TBL", "StudyId", "Study_TBL", "StudyId", cascadeDelete: true);
            //AddForeignKey("Activity_TBL", "StudyId", "Study_TBL", "StudyId", cascadeDelete: true);
            //DropColumn("Project_TBL", "ShortName");
        }
        
        public override void Down()
        {
            AddColumn("Project_TBL", "ShortName", c => c.String(unicode: false));
            DropForeignKey("Activity_TBL", "StudyId", "Study_TBL");
            DropForeignKey("Visit_TBL", "StudyId", "Study_TBL");
            DropForeignKey("Study_Observations", "StudyId", "Study_TBL");
            DropForeignKey("Subject_TBL", "StudyId", "Study_TBL");
            DropForeignKey("BioSamples_TBL", "StudyId", "Study_TBL");
            DropForeignKey("Variable_Definition_TBL", "StudyId", "Study_TBL");
            DropIndex("Study_Observations", new[] { "StudyId" });
            DropIndex("Subject_TBL", new[] { "StudyId" });
            DropIndex("BioSamples_TBL", new[] { "StudyId" });
            DropIndex("Visit_TBL", new[] { "StudyId" });
            DropIndex("Variable_Definition_TBL", new[] { "StudyId" });
            DropIndex("Activity_TBL", new[] { "StudyId" });
            DropPrimaryKey("Study_Observations");
            DropPrimaryKey("Study_TBL");
            AlterColumn("Study_Observations", "StudyId", c => c.String(nullable: false, maxLength: 200, storeType: "nvarchar"));
            AlterColumn("Subject_TBL", "StudyId", c => c.String(maxLength: 200, storeType: "nvarchar"));
            AlterColumn("BioSamples_TBL", "StudyId", c => c.String(maxLength: 200, storeType: "nvarchar"));
            AlterColumn("Visit_TBL", "StudyId", c => c.String(nullable: false, maxLength: 200, storeType: "nvarchar"));
            AlterColumn("Variable_Definition_TBL", "StudyId", c => c.String(maxLength: 200, storeType: "nvarchar"));
            AlterColumn("Study_TBL", "StudyId", c => c.String(nullable: false, maxLength: 200, storeType: "nvarchar"));
            AlterColumn("Activity_TBL", "StudyId", c => c.String(nullable: false, maxLength: 200, storeType: "nvarchar"));
            DropColumn("Characteristics_TBL", "ControlledTermStr");
            DropColumn("Study_TBL", "Accession");
            AddPrimaryKey("Study_Observations", new[] { "StudyId", "ObservationId" });
            AddPrimaryKey("Study_TBL", "OID");
            RenameColumn(table: "Study_TBL", name: "StudyId", newName: "OID");
            CreateIndex("Study_Observations", "StudyId");
            CreateIndex("Subject_TBL", "StudyId");
            CreateIndex("BioSamples_TBL", "StudyId");
            CreateIndex("Visit_TBL", "StudyId");
            CreateIndex("Variable_Definition_TBL", "StudyId");
            CreateIndex("Activity_TBL", "StudyId");
            AddForeignKey("Activity_TBL", "StudyId", "Study_TBL", "StudyId", cascadeDelete: true);
            AddForeignKey("Visit_TBL", "StudyId", "Study_TBL", "StudyId", cascadeDelete: true);
            AddForeignKey("Study_Observations", "StudyId", "Study_TBL", "StudyId", cascadeDelete: true);
            AddForeignKey("Subject_TBL", "StudyId", "Study_TBL", "OID");
            AddForeignKey("BioSamples_TBL", "StudyId", "Study_TBL", "OID");
            AddForeignKey("Variable_Definition_TBL", "StudyId", "Study_TBL", "OID");
        }
    }
}
