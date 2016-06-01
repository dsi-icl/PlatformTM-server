namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addassay : DbMigration
    {
        public override void Up()
        {
            AddColumn("Activity_TBL", "Discriminator", c => c.String(nullable: false, maxLength: 128, storeType: "nvarchar"));
            AddColumn("Activity_TBL", "DesignType_Id", c => c.String(maxLength: 200, storeType: "nvarchar"));
            AddColumn("Activity_TBL", "MeasurementType_Id", c => c.String(maxLength: 200, storeType: "nvarchar"));
            AddColumn("Activity_TBL", "TechnologyPlatform_Id", c => c.String(maxLength: 200, storeType: "nvarchar"));
            AddColumn("Activity_TBL", "TechnologyType_Id", c => c.String(maxLength: 200, storeType: "nvarchar"));
            CreateIndex("Activity_TBL", "DesignType_Id");
            CreateIndex("Activity_TBL", "MeasurementType_Id");
            CreateIndex("Activity_TBL", "TechnologyPlatform_Id");
            CreateIndex("Activity_TBL", "TechnologyType_Id");
            AddForeignKey("Activity_TBL", "DesignType_Id", "CVterm_TBL", "OID");
            AddForeignKey("Activity_TBL", "MeasurementType_Id", "CVterm_TBL", "OID");
            AddForeignKey("Activity_TBL", "TechnologyPlatform_Id", "CVterm_TBL", "OID");
            AddForeignKey("Activity_TBL", "TechnologyType_Id", "CVterm_TBL", "OID");
        }
        
        public override void Down()
        {
            DropForeignKey("Activity_TBL", "TechnologyType_Id", "CVterm_TBL");
            DropForeignKey("Activity_TBL", "TechnologyPlatform_Id", "CVterm_TBL");
            DropForeignKey("Activity_TBL", "MeasurementType_Id", "CVterm_TBL");
            DropForeignKey("Activity_TBL", "DesignType_Id", "CVterm_TBL");
            DropIndex("Activity_TBL", new[] { "TechnologyType_Id" });
            DropIndex("Activity_TBL", new[] { "TechnologyPlatform_Id" });
            DropIndex("Activity_TBL", new[] { "MeasurementType_Id" });
            DropIndex("Activity_TBL", new[] { "DesignType_Id" });
            DropColumn("Activity_TBL", "TechnologyType_Id");
            DropColumn("Activity_TBL", "TechnologyPlatform_Id");
            DropColumn("Activity_TBL", "MeasurementType_Id");
            DropColumn("Activity_TBL", "DesignType_Id");
            DropColumn("Activity_TBL", "Discriminator");
        }
    }
}
