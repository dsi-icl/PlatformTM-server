namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remCharacteristicsobjTBL : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("CharacteristicObjects", "CVtermId", "CVterm_TBL");
            DropForeignKey("CharacteristicObjects", "ProjectId", "Project_TBL");
            DropForeignKey("Characteristics_TBL", "CharacteristicObjectId", "CharacteristicObjects");
            DropIndex("Characteristics_TBL", new[] { "CharacteristicObjectId" });
            DropIndex("CharacteristicObjects", new[] { "CVtermId" });
            DropIndex("CharacteristicObjects", new[] { "ProjectId" });
            DropColumn("Characteristics_TBL", "CharacteristicObjectId");
            DropTable("CharacteristicObjects");
        }
        
        public override void Down()
        {
            CreateTable(
                "CharacteristicObjects",
                c => new
                    {
                        CharacteristicObjId = c.Int(nullable: false, identity: true),
                        ShortName = c.String(unicode: false),
                        FullName = c.String(unicode: false),
                        Domain = c.String(unicode: false),
                        CVtermId = c.String(maxLength: 200, storeType: "nvarchar"),
                        ProjectId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CharacteristicObjId);
            
            AddColumn("Characteristics_TBL", "CharacteristicObjectId", c => c.Int(nullable: false));
            CreateIndex("CharacteristicObjects", "ProjectId");
            CreateIndex("CharacteristicObjects", "CVtermId");
            CreateIndex("Characteristics_TBL", "CharacteristicObjectId");
            AddForeignKey("Characteristics_TBL", "CharacteristicObjectId", "CharacteristicObjects", "CharacteristicObjId", cascadeDelete: true);
            AddForeignKey("CharacteristicObjects", "ProjectId", "Project_TBL", "ProjectId", cascadeDelete: true);
            AddForeignKey("CharacteristicObjects", "CVtermId", "CVterm_TBL", "OID");
        }
    }
}
