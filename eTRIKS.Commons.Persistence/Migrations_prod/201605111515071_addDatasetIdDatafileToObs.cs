namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDatasetIdDatafileToObs : DbMigration
    {
        public override void Up()
        {
            AddColumn("Observation_TBL", "DatasetId", c => c.Int());
            AddColumn("Observation_TBL", "DatafileId", c => c.Int());
            CreateIndex("Observation_TBL", "DatasetId");
            CreateIndex("Observation_TBL", "DatafileId");
            AddForeignKey("Observation_TBL", "DatafileId", "DataFiles_TBL", "DataFileId");
            AddForeignKey("Observation_TBL", "DatasetId", "Dataset_TBL", "OID");
        }
        
        public override void Down()
        {
            DropForeignKey("Observation_TBL", "DatasetId", "Dataset_TBL");
            DropForeignKey("Observation_TBL", "DatafileId", "DataFiles_TBL");
            DropIndex("Observation_TBL", new[] { "DatafileId" });
            DropIndex("Observation_TBL", new[] { "DatasetId" });
            DropColumn("Observation_TBL", "DatafileId");
            DropColumn("Observation_TBL", "DatasetId");
        }
    }
}
