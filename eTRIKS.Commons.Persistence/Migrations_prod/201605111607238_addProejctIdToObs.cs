namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addProejctIdToObs : DbMigration
    {
        public override void Up()
        {
            AddColumn("Observation_TBL", "ProjectId", c => c.Int());
            CreateIndex("Observation_TBL", "ProjectId");
            AddForeignKey("Observation_TBL", "ProjectId", "Project_TBL", "ProjectId");
        }
        
        public override void Down()
        {
            DropForeignKey("Observation_TBL", "ProjectId", "Project_TBL");
            DropIndex("Observation_TBL", new[] { "ProjectId" });
            DropColumn("Observation_TBL", "ProjectId");
        }
    }
}
