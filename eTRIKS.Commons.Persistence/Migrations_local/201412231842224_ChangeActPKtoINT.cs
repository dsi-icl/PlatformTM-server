namespace eTRIKS.Commons.Persistence.Migrations_local
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeActPKtoINT : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Dataset_TBL", "ActivityId", "Activity_TBL");
            DropIndex("Dataset_TBL", new[] { "ActivityId" });
            RenameColumn(table: "Activity_TBL", name: "OID", newName: "ActivityId");
            RenameColumn(table: "Variable_Reference_TBL", name: "DatasetId", newName: "ActivityDatasetId");
            RenameIndex(table: "Variable_Reference_TBL", name: "IX_DatasetId", newName: "IX_ActivityDatasetId");
            DropPrimaryKey("Activity_TBL");
            AddPrimaryKey("Activity_TBL", "ActivityId");
            AlterColumn("Dataset_TBL", "ActivityId", c => c.Int(nullable: false));
            AlterColumn("Activity_TBL", "ActivityId", c => c.Int(nullable: false, identity: true));

            CreateIndex("Dataset_TBL", "ActivityId");
            AddForeignKey("Dataset_TBL", "ActivityId", "Activity_TBL", "ActivityId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("Dataset_TBL", "ActivityId", "Activity_TBL");
            DropIndex("Dataset_TBL", new[] { "ActivityId" });
            DropPrimaryKey("Activity_TBL");
            AlterColumn("Activity_TBL", "ActivityId", c => c.String(nullable: false, maxLength: 200, storeType: "nvarchar"));
            AlterColumn("Dataset_TBL", "ActivityId", c => c.String(nullable: false, maxLength: 200, storeType: "nvarchar"));
            AddPrimaryKey("Activity_TBL", "OID");
            RenameIndex(table: "Variable_Reference_TBL", name: "IX_ActivityDatasetId", newName: "IX_DatasetId");
            RenameColumn(table: "Variable_Reference_TBL", name: "ActivityDatasetId", newName: "DatasetId");
            RenameColumn(table: "Activity_TBL", name: "ActivityId", newName: "OID");
            CreateIndex("Dataset_TBL", "ActivityId");
            AddForeignKey("Dataset_TBL", "ActivityId", "Activity_TBL", "ActivityId", cascadeDelete: true);
        }
    }
}
