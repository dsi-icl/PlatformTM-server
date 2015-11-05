namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDatasetTBL : DbMigration
    {
        public override void Up()
        {
            AddColumn("Dataset_TBL", "StandardDataFile", c => c.String(unicode: false));
            AddColumn("Dataset_TBL", "State", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("Dataset_TBL", "State");
            DropColumn("Dataset_TBL", "StandardDataFile");
        }
    }
}
