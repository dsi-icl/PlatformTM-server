namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class delCharacteristicsTBL : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.Characteristics_TBL", "SampleId", "dbo.BioSamples_TBL");
            //DropForeignKey("dbo.Characteristics_TBL", "SubjectId", "dbo.Subject_TBL");
            //DropForeignKey("dbo.Characteristics_TBL", "CVtermId", "dbo.CVterm_TBL");
            //DropForeignKey("dbo.Characteristics_TBL", new[] { "DatasetVariableId", "DatasetId" }, "dbo.Variable_Reference_TBL");
            //DropIndex("dbo.Characteristics_TBL", new[] { "CVtermId" });
            //DropIndex("dbo.Characteristics_TBL", new[] { "DatasetVariableId", "DatasetId" });
            //DropIndex("dbo.Characteristics_TBL", new[] { "SampleId" });
            //DropIndex("dbo.Characteristics_TBL", new[] { "SubjectId" });
            //DropTable("dbo.Characteristics_TBL");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Characteristics_TBL",
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
                .PrimaryKey(t => t.CharacterisitcId);
            
            CreateIndex("dbo.Characteristics_TBL", "SubjectId");
            CreateIndex("dbo.Characteristics_TBL", "SampleId");
            CreateIndex("dbo.Characteristics_TBL", new[] { "DatasetVariableId", "DatasetId" });
            CreateIndex("dbo.Characteristics_TBL", "CVtermId");
            AddForeignKey("dbo.Characteristics_TBL", new[] { "DatasetVariableId", "DatasetId" }, "dbo.Variable_Reference_TBL", new[] { "VariableId", "ActivityDatasetId" }, cascadeDelete: true);
            AddForeignKey("dbo.Characteristics_TBL", "CVtermId", "dbo.CVterm_TBL", "OID");
            AddForeignKey("dbo.Characteristics_TBL", "SubjectId", "dbo.Subject_TBL", "SubjectDBId");
            AddForeignKey("dbo.Characteristics_TBL", "SampleId", "dbo.BioSamples_TBL", "BioSampleDBId", cascadeDelete: true);
        }
    }
}
