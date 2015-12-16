namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addColToDAtafile : DbMigration
    {
        public override void Up()
        {
            AddColumn("DataFiles_TBL", "LoadedToDB", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("DataFiles_TBL", "LoadedToDB");
        }
    }
}
