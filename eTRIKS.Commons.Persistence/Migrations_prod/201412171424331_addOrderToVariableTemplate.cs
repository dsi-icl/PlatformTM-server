namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addOrderToVariableTemplate : DbMigration
    {
        public override void Up()
        {
            //RenameColumn(table: "dbo.Variable_Reference_TBL", name: "ActivityDatasetId", newName: "DatasetId");
            //RenameIndex(table: "dbo.Variable_Reference_TBL", name: "IX_ActivityDatasetId", newName: "IX_DatasetId");
            AddColumn("Templates.DomainVariable_TBL", "Order", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Templates.DomainVariable_TBL", "Order");
            //RenameIndex(table: "dbo.Variable_Reference_TBL", name: "IX_DatasetId", newName: "IX_ActivityDatasetId");
            //RenameColumn(table: "dbo.Variable_Reference_TBL", name: "DatasetId", newName: "ActivityDatasetId");
        }
    }
}
