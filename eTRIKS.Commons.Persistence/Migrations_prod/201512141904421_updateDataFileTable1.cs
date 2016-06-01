namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDataFileTable1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("DataFiles_TBL", "IsStandard", c => c.Boolean(nullable: false));
            DropColumn("Dataset_TBL", "StandardDataFile");
        }
        
        public override void Down()
        {
            AddColumn("Dataset_TBL", "StandardDataFile", c => c.String(unicode: false));
            DropColumn("DataFiles_TBL", "IsStandard");
        }
    }
}
