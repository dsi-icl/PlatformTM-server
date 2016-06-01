namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCharacteristicsTBL : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Characteristics_TBL",
                c => new
                    {
                        CharacterisitcId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 2000, storeType: "nvarchar"),
                        Value = c.String(unicode: false),
                        ControlledTermStr = c.String(unicode: false),
                        CVtermId = c.String(maxLength: 200, storeType: "nvarchar"),
                        DatasetDomainCode = c.String(unicode: false),
                        DatasetVariableId = c.Int(nullable: false),
                        DatasetId = c.Int(nullable: false),
                        SampleId = c.Int(),
                        SubjectId = c.String(maxLength: 128, storeType: "nvarchar"),
                        Discriminator = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.CharacterisitcId)
                .ForeignKey("BioSamples_TBL", t => t.SampleId, cascadeDelete: true)
                .ForeignKey("Subject_TBL", t => t.SubjectId)
                .ForeignKey("CVterm_TBL", t => t.CVtermId)
                .ForeignKey("Variable_Reference_TBL", t => new { t.DatasetVariableId, t.DatasetId }, cascadeDelete: true)
                .Index(t => t.CVtermId)
                .Index(t => new { t.DatasetVariableId, t.DatasetId })
                .Index(t => t.SampleId)
                .Index(t => t.SubjectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Characteristics_TBL", new[] { "DatasetVariableId", "DatasetId" }, "Variable_Reference_TBL");
            DropForeignKey("Characteristics_TBL", "CVtermId", "CVterm_TBL");
            DropForeignKey("Characteristics_TBL", "SubjectId", "Subject_TBL");
            DropForeignKey("Characteristics_TBL", "SampleId", "BioSamples_TBL");
            DropIndex("Characteristics_TBL", new[] { "SubjectId" });
            DropIndex("Characteristics_TBL", new[] { "SampleId" });
            DropIndex("Characteristics_TBL", new[] { "DatasetVariableId", "DatasetId" });
            DropIndex("Characteristics_TBL", new[] { "CVtermId" });
            DropTable("Characteristics_TBL");
        }
    }
}
