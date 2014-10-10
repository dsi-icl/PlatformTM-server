namespace eTRIKS.Commons.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testUpdate2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CVterm_TBL", "Synonyms", c => c.String(maxLength: 2000, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CVterm_TBL", "Synonyms");
        }
    }
}
