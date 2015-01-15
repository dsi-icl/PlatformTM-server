namespace eTRIKS.Commons.Persistence.Migrations_local
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PKtoINT_Dataset_VarDEF : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Variable_Reference_TBL", "ActivityDatasetId", "Dataset_TBL");
            DropForeignKey("Variable_Reference_TBL", "VariableId", "Variable_Definition_TBL");
            DropForeignKey("Derived_Method_TBL", "DerivedVariable_OID", "Variable_Definition_TBL");
            DropIndex("Variable_Reference_TBL", new[] { "VariableId" });
            DropIndex("Variable_Reference_TBL", new[] { "ActivityDatasetId" });
            DropIndex("Derived_Method_TBL", new[] { "DerivedVariable_OID" });
            DropPrimaryKey("Dataset_TBL");
            DropPrimaryKey("Variable_Reference_TBL");
            DropPrimaryKey("Variable_Definition_TBL");
            AddColumn("Variable_Definition_TBL", "Accession", c => c.String(unicode: false));
            AlterColumn("Dataset_TBL", "OID", c => c.Int(nullable: false, identity: true));
            AlterColumn("Variable_Reference_TBL", "VariableId", c => c.Int(nullable: false));
            AlterColumn("Variable_Reference_TBL", "ActivityDatasetId", c => c.Int(nullable: false));
            AlterColumn("Variable_Definition_TBL", "OID", c => c.Int(nullable: false, identity: true));
            AlterColumn("Derived_Method_TBL", "DerivedVariable_OID", c => c.Int());
            AddPrimaryKey("Dataset_TBL", "OID");
            AddPrimaryKey("Variable_Reference_TBL", new[] { "VariableId", "ActivityDatasetId" });
            AddPrimaryKey("Variable_Definition_TBL", "OID");
            CreateIndex("Variable_Reference_TBL", "VariableId");
            CreateIndex("Variable_Reference_TBL", "ActivityDatasetId");
            CreateIndex("Derived_Method_TBL", "DerivedVariable_OID");
            AddForeignKey("Variable_Reference_TBL", "ActivityDatasetId", "Dataset_TBL", "OID", cascadeDelete: true);
            AddForeignKey("Variable_Reference_TBL", "VariableId", "Variable_Definition_TBL", "OID", cascadeDelete: true);
            AddForeignKey("Derived_Method_TBL", "DerivedVariable_OID", "Variable_Definition_TBL", "OID");
        }
        
        public override void Down()
        {
            DropForeignKey("Derived_Method_TBL", "DerivedVariable_OID", "Variable_Definition_TBL");
            DropForeignKey("Variable_Reference_TBL", "VariableId", "Variable_Definition_TBL");
            DropForeignKey("Variable_Reference_TBL", "ActivityDatasetId", "Dataset_TBL");
            DropIndex("Derived_Method_TBL", new[] { "DerivedVariable_OID" });
            DropIndex("Variable_Reference_TBL", new[] { "ActivityDatasetId" });
            DropIndex("Variable_Reference_TBL", new[] { "VariableId" });
            DropPrimaryKey("Variable_Definition_TBL");
            DropPrimaryKey("Variable_Reference_TBL");
            DropPrimaryKey("Dataset_TBL");
            AlterColumn("Derived_Method_TBL", "DerivedVariable_OID", c => c.String(maxLength: 200, storeType: "nvarchar"));
            AlterColumn("Variable_Definition_TBL", "OID", c => c.String(nullable: false, maxLength: 200, storeType: "nvarchar"));
            AlterColumn("Variable_Reference_TBL", "ActivityDatasetId", c => c.String(nullable: false, maxLength: 200, storeType: "nvarchar"));
            AlterColumn("Variable_Reference_TBL", "VariableId", c => c.String(nullable: false, maxLength: 200, storeType: "nvarchar"));
            AlterColumn("Dataset_TBL", "OID", c => c.String(nullable: false, maxLength: 200, storeType: "nvarchar"));
            DropColumn("Variable_Definition_TBL", "Accession");
            AddPrimaryKey("Variable_Definition_TBL", "OID");
            AddPrimaryKey("Variable_Reference_TBL", new[] { "VariableId", "ActivityDatasetId" });
            AddPrimaryKey("Dataset_TBL", "OID");
            CreateIndex("Derived_Method_TBL", "DerivedVariable_OID");
            CreateIndex("Variable_Reference_TBL", "ActivityDatasetId");
            CreateIndex("Variable_Reference_TBL", "VariableId");
            AddForeignKey("Derived_Method_TBL", "DerivedVariable_OID", "Variable_Definition_TBL", "OID");
            AddForeignKey("Variable_Reference_TBL", "VariableId", "Variable_Definition_TBL", "OID", cascadeDelete: true);
            AddForeignKey("Variable_Reference_TBL", "ActivityDatasetId", "Dataset_TBL", "OID", cascadeDelete: true);
        }
    }
}
