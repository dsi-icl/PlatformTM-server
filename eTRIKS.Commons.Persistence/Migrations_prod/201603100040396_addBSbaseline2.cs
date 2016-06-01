namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addBSbaseline2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BioSamples_TBL", "IsBaseline", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BioSamples_TBL", "IsBaseline", c => c.Boolean(nullable: false));
        }
    }
}
