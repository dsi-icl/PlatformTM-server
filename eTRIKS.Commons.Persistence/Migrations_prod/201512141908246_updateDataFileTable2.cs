namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDataFileTable2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("Dataset_TBL", "DataFile");
        }
        
        public override void Down()
        {
            AddColumn("Dataset_TBL", "DataFile", c => c.String(maxLength: 2000, storeType: "nvarchar"));
        }
    }
}
