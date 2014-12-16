namespace eTRIKS.Commons.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingVarCVFK : DbMigration
    {
        public override void Up()
        {
//            DropForeignKey("Templates.DomainVariable_TBL", "DomainId", "Templates.DomainTemplate_TBL");
//            DropIndex("Templates.DomainVariable_TBL", new[] { "DomainId" });

//            Sql(@"SET FOREIGN_KEY_CHECKS = 0;
//             ALTER TABLE CVterm_TBL DROP INDEX DictionartyId;
//             ALTER TABLE CVterm_TBL ADD INDEX IX_DictionaryId (DictionaryId);");
            
//            RenameColumn(table: "CVterm_TBL", name: "DictionartyId", newName: "DictionaryId");
//            //RenameIndex(table: "dbo.CVterm_TBL", name: "IX_DictionartyId", newName: "IX_DictionaryId");
//            AddColumn("Templates.DomainVariable_TBL", "DictionaryId", c => c.String(maxLength: 200, storeType: "nvarchar"));
//            AlterColumn("Templates.DomainVariable_TBL", "DomainId", c => c.String(nullable: false, maxLength: 200, storeType: "nvarchar"));
//            CreateIndex("Templates.DomainVariable_TBL", "DomainId");
//            CreateIndex("Templates.DomainVariable_TBL", "DictionaryId");
//            AddForeignKey("Templates.DomainVariable_TBL", "DictionaryId", "dbo.Dictionary_TBL", "OID");
//            AddForeignKey("Templates.DomainVariable_TBL", "DomainId", "Templates.DomainTemplate_TBL", "OID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("Templates.DomainVariable_TBL", "DomainId", "Templates.DomainTemplate_TBL");
            DropForeignKey("Templates.DomainVariable_TBL", "DictionaryId", "dbo.Dictionary_TBL");
            DropIndex("Templates.DomainVariable_TBL", new[] { "DictionaryId" });
            DropIndex("Templates.DomainVariable_TBL", new[] { "DomainId" });
            AlterColumn("Templates.DomainVariable_TBL", "DomainId", c => c.String(maxLength: 200, storeType: "nvarchar"));
            DropColumn("Templates.DomainVariable_TBL", "DictionaryId");
            RenameIndex(table: "dbo.CVterm_TBL", name: "IX_DictionaryId", newName: "IX_DictionartyId");
            RenameColumn(table: "dbo.CVterm_TBL", name: "DictionaryId", newName: "DictionartyId");
            CreateIndex("Templates.DomainVariable_TBL", "DomainId");
            AddForeignKey("Templates.DomainVariable_TBL", "DomainId", "Templates.DomainTemplate_TBL", "OID");
        }
    }
}
