namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAssayTBL : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "Activity_TBL", name: "DesignType_Id", newName: "DesignTypeId");
            RenameColumn(table: "Activity_TBL", name: "MeasurementType_Id", newName: "MeasurementTypeId");
            RenameColumn(table: "Activity_TBL", name: "TechnologyPlatform_Id", newName: "TechnologyPlatformId");
            RenameColumn(table: "Activity_TBL", name: "TechnologyType_Id", newName: "TechnologyTypeId");
            //RenameIndex(table: "Activity_TBL", name: "IX_DesignType_Id", newName: "IX_DesignTypeId");
            //RenameIndex(table: "Activity_TBL", name: "IX_TechnologyType_Id", newName: "IX_TechnologyTypeId");
            //RenameIndex(table: "Activity_TBL", name: "IX_TechnologyPlatform_Id", newName: "IX_TechnologyPlatformId");
            //RenameIndex(table: "Activity_TBL", name: "IX_MeasurementType_Id", newName: "IX_MeasurementTypeId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "Activity_TBL", name: "IX_MeasurementTypeId", newName: "IX_MeasurementType_Id");
            RenameIndex(table: "Activity_TBL", name: "IX_TechnologyPlatformId", newName: "IX_TechnologyPlatform_Id");
            RenameIndex(table: "Activity_TBL", name: "IX_TechnologyTypeId", newName: "IX_TechnologyType_Id");
            RenameIndex(table: "Activity_TBL", name: "IX_DesignTypeId", newName: "IX_DesignType_Id");
            RenameColumn(table: "Activity_TBL", name: "TechnologyTypeId", newName: "TechnologyType_Id");
            RenameColumn(table: "Activity_TBL", name: "TechnologyPlatformId", newName: "TechnologyPlatform_Id");
            RenameColumn(table: "Activity_TBL", name: "MeasurementTypeId", newName: "MeasurementType_Id");
            RenameColumn(table: "Activity_TBL", name: "DesignTypeId", newName: "DesignType_Id");
        }
    }
}
