namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldToObservation : DbMigration
    {
        public override void Up()
        {
            AddColumn("Observation_TBL", "isSubjCharacteristic", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("Observation_TBL", "isSubjCharacteristic");
        }
    }
}
