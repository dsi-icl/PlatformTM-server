namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PKtoINT_Dataset_VarDEF : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Variable_Reference_TBL", "ActivityDatasetId", "dbo.Dataset_TBL");
            DropForeignKey("dbo.Dataset_TBL", "ActivityId", "dbo.Activity_TBL");
            DropForeignKey("dbo.Variable_Reference_TBL", "VariableId", "dbo.Variable_Definition_TBL");
            DropForeignKey("dbo.Derived_Method_TBL", "DerivedVariable_OID", "dbo.Variable_Definition_TBL");
            DropIndex("dbo.Dataset_TBL", new[] { "ActivityId" });
            DropIndex("dbo.Variable_Reference_TBL", new[] { "VariableId" });
            DropIndex("dbo.Variable_Reference_TBL", new[] { "DatasetId" });
            DropIndex("dbo.Derived_Method_TBL", new[] { "DerivedVariable_OID" });
            RenameColumn(table: "dbo.Activity_TBL", name: "OID", newName: "ActivityId");
            RenameColumn(table: "dbo.Variable_Reference_TBL", name: "DatasetId", newName: "ActivityDatasetId");
            DropPrimaryKey("dbo.Dataset_TBL");
            DropPrimaryKey("dbo.Activity_TBL");
            DropPrimaryKey("dbo.Variable_Reference_TBL");
            DropPrimaryKey("dbo.Variable_Definition_TBL");
            AddColumn("dbo.Variable_Definition_TBL", "Accession", c => c.String(unicode: false));
            AlterColumn("dbo.Dataset_TBL", "OID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Dataset_TBL", "ActivityId", c => c.Int(nullable: false));
            AlterColumn("dbo.Activity_TBL", "ActivityId", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Variable_Reference_TBL", "VariableId", c => c.Int(nullable: false));
            AlterColumn("dbo.Variable_Reference_TBL", "ActivityDatasetId", c => c.Int(nullable: false));
            AlterColumn("dbo.Variable_Definition_TBL", "OID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Derived_Method_TBL", "DerivedVariable_OID", c => c.Int());
            AddPrimaryKey("dbo.Dataset_TBL", "OID");
            AddPrimaryKey("dbo.Activity_TBL", "ActivityId");
            AddPrimaryKey("dbo.Variable_Reference_TBL", new[] { "VariableId", "ActivityDatasetId" });
            AddPrimaryKey("dbo.Variable_Definition_TBL", "OID");
            CreateIndex("dbo.Dataset_TBL", "ActivityId");
            CreateIndex("dbo.Variable_Reference_TBL", "VariableId");
            CreateIndex("dbo.Variable_Reference_TBL", "ActivityDatasetId");
            CreateIndex("dbo.Derived_Method_TBL", "DerivedVariable_OID");
            AddForeignKey("dbo.Variable_Reference_TBL", "ActivityDatasetId", "dbo.Dataset_TBL", "OID", cascadeDelete: true);
            AddForeignKey("dbo.Dataset_TBL", "ActivityId", "dbo.Activity_TBL", "ActivityId", cascadeDelete: true);
            AddForeignKey("dbo.Variable_Reference_TBL", "VariableId", "dbo.Variable_Definition_TBL", "OID", cascadeDelete: true);
            AddForeignKey("dbo.Derived_Method_TBL", "DerivedVariable_OID", "dbo.Variable_Definition_TBL", "OID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Derived_Method_TBL", "DerivedVariable_OID", "dbo.Variable_Definition_TBL");
            DropForeignKey("dbo.Variable_Reference_TBL", "VariableId", "dbo.Variable_Definition_TBL");
            DropForeignKey("dbo.Dataset_TBL", "ActivityId", "dbo.Activity_TBL");
            DropForeignKey("dbo.Variable_Reference_TBL", "ActivityDatasetId", "dbo.Dataset_TBL");
            DropIndex("dbo.Derived_Method_TBL", new[] { "DerivedVariable_OID" });
            DropIndex("dbo.Variable_Reference_TBL", new[] { "ActivityDatasetId" });
            DropIndex("dbo.Variable_Reference_TBL", new[] { "VariableId" });
            DropIndex("dbo.Dataset_TBL", new[] { "ActivityId" });
            DropPrimaryKey("dbo.Variable_Definition_TBL");
            DropPrimaryKey("dbo.Variable_Reference_TBL");
            DropPrimaryKey("dbo.Activity_TBL");
            DropPrimaryKey("dbo.Dataset_TBL");
            AlterColumn("dbo.Derived_Method_TBL", "DerivedVariable_OID", c => c.String(maxLength: 200, storeType: "nvarchar"));
            AlterColumn("dbo.Variable_Definition_TBL", "OID", c => c.String(nullable: false, maxLength: 200, storeType: "nvarchar"));
            AlterColumn("dbo.Variable_Reference_TBL", "ActivityDatasetId", c => c.String(nullable: false, maxLength: 200, storeType: "nvarchar"));
            AlterColumn("dbo.Variable_Reference_TBL", "VariableId", c => c.String(nullable: false, maxLength: 200, storeType: "nvarchar"));
            AlterColumn("dbo.Activity_TBL", "ActivityId", c => c.String(nullable: false, maxLength: 200, storeType: "nvarchar"));
            AlterColumn("dbo.Dataset_TBL", "ActivityId", c => c.String(nullable: false, maxLength: 200, storeType: "nvarchar"));
            AlterColumn("dbo.Dataset_TBL", "OID", c => c.String(nullable: false, maxLength: 200, storeType: "nvarchar"));
            DropColumn("dbo.Variable_Definition_TBL", "Accession");
            AddPrimaryKey("dbo.Variable_Definition_TBL", "OID");
            AddPrimaryKey("dbo.Variable_Reference_TBL", new[] { "VariableId", "DatasetId" });
            AddPrimaryKey("dbo.Activity_TBL", "OID");
            AddPrimaryKey("dbo.Dataset_TBL", "OID");
            RenameColumn(table: "dbo.Variable_Reference_TBL", name: "ActivityDatasetId", newName: "DatasetId");
            RenameColumn(table: "dbo.Activity_TBL", name: "ActivityId", newName: "OID");
            CreateIndex("dbo.Derived_Method_TBL", "DerivedVariable_OID");
            CreateIndex("dbo.Variable_Reference_TBL", "DatasetId");
            CreateIndex("dbo.Variable_Reference_TBL", "VariableId");
            CreateIndex("dbo.Dataset_TBL", "ActivityId");
            AddForeignKey("dbo.Derived_Method_TBL", "DerivedVariable_OID", "dbo.Variable_Definition_TBL", "OID");
            AddForeignKey("dbo.Variable_Reference_TBL", "VariableId", "dbo.Variable_Definition_TBL", "OID", cascadeDelete: true);
            AddForeignKey("dbo.Dataset_TBL", "ActivityId", "dbo.Activity_TBL", "ActivityId", cascadeDelete: true);
            AddForeignKey("dbo.Variable_Reference_TBL", "ActivityDatasetId", "dbo.Dataset_TBL", "OID", cascadeDelete: true);
        }
    }
}
