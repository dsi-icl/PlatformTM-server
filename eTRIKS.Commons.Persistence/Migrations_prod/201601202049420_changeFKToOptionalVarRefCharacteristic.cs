namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeFKToOptionalVarRefCharacteristic : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Characteristics_TBL", new[] { "DatasetVariableId", "DatasetId" }, "Variable_Reference_TBL");
            AddForeignKey("Characteristics_TBL", new[] { "DatasetVariableId", "DatasetId" }, "Variable_Reference_TBL", new[] { "VariableId", "ActivityDatasetId" }, cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("Characteristics_TBL", new[] { "DatasetVariableId", "DatasetId" }, "Variable_Reference_TBL");
            AddForeignKey("Characteristics_TBL", new[] { "DatasetVariableId", "DatasetId" }, "Variable_Reference_TBL", new[] { "VariableId", "ActivityDatasetId" });
        }
    }
}
