namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDomainNameToObs : DbMigration
    {
        public override void Up()
        {
            AddColumn("Derived_Method_TBL", "type", c => c.String(maxLength: 200, storeType: "nvarchar"));
            //DropColumn("Derived_Method_TBL", "type");
            //RenameColumn(table: "Derived_Method_TBL", name: "DerivedValueType_OID", newName: "type");
            //RenameColumn(table: "Derived_Method_TBL", name: "DerivedVariable_OID", newName: "DerivedVariable_Id");
            //RenameColumn(table: "Variable_Definition_TBL", name: "DerivedMethod_OID", newName: "DerivedMethod_Id");
            //RenameIndex(table: "Variable_Definition_TBL", name: "IX_DerivedMethod_OID", newName: "IX_DerivedMethod_Id");
            //RenameIndex(table: "Derived_Method_TBL", name: "IX_DerivedValueType_OID", newName: "IX_type");
            //RenameIndex(table: "Derived_Method_TBL", name: "IX_DerivedVariable_OID", newName: "IX_DerivedVariable_Id");
            AddColumn("Observation_TBL", "DomainName", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("Observation_TBL", "DomainName");
            RenameIndex(table: "Derived_Method_TBL", name: "IX_DerivedVariable_Id", newName: "IX_DerivedVariable_OID");
            RenameIndex(table: "Derived_Method_TBL", name: "IX_type", newName: "IX_DerivedValueType_OID");
            RenameIndex(table: "Variable_Definition_TBL", name: "IX_DerivedMethod_Id", newName: "IX_DerivedMethod_OID");
            RenameColumn(table: "Variable_Definition_TBL", name: "DerivedMethod_Id", newName: "DerivedMethod_OID");
            RenameColumn(table: "Derived_Method_TBL", name: "DerivedVariable_Id", newName: "DerivedVariable_OID");
            RenameColumn(table: "Derived_Method_TBL", name: "type", newName: "DerivedValueType_OID");
            AddColumn("Derived_Method_TBL", "type", c => c.String(maxLength: 200, storeType: "nvarchar"));
        }
    }
}
