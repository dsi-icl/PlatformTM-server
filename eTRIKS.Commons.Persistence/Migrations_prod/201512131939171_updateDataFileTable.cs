namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDataFileTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("DataFiles_TBL", "Path", c => c.String(unicode: false));
            AddColumn("DataFiles_TBL", "IsDirectory", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("DataFiles_TBL", "IsDirectory");
            DropColumn("DataFiles_TBL", "Path");
        }
    }
}
