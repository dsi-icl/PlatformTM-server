namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCharacteristicsTBL1 : DbMigration
    {
        public override void Up()
        {
            //AddColumn("CharacteristicObjects", "ShortName", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("CharacteristicObjects", "ShortName");
        }
    }
}
