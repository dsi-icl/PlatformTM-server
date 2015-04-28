namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addobservation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Observation_TBL",
                c => new
                    {
                        ObservationId = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        ControlledTermStr = c.String(unicode: false),
                        ControlledTermId = c.String(maxLength: 200, storeType: "nvarchar"),
                        DomainCode = c.String(unicode: false),
                        TopicVariableId = c.Int(nullable: false),
                        Class = c.String(unicode: false),
                        Group = c.String(unicode: false),
                        Subgroup = c.String(unicode: false),
                        DefaultQualifierId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ObservationId)
                .ForeignKey("CVterm_TBL", t => t.ControlledTermId)
                .ForeignKey("Variable_Definition_TBL", t => t.DefaultQualifierId, cascadeDelete: true)
                .ForeignKey("Variable_Definition_TBL", t => t.TopicVariableId, cascadeDelete: true)
                .Index(t => t.ControlledTermId)
                .Index(t => t.TopicVariableId)
                .Index(t => t.DefaultQualifierId);
            
            CreateTable(
                "Variable_Qualifiers",
                c => new
                    {
                        VariableId = c.Int(nullable: false),
                        QualifierId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.VariableId, t.QualifierId })
                .ForeignKey("Variable_Definition_TBL", t => t.VariableId)
                .ForeignKey("Variable_Definition_TBL", t => t.QualifierId)
                .Index(t => t.VariableId)
                .Index(t => t.QualifierId);
            
            CreateTable(
                "Observation_Qualfiers",
                c => new
                    {
                        ObservationId = c.Int(nullable: false),
                        QualifierId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ObservationId, t.QualifierId })
                .ForeignKey("Observation_TBL", t => t.ObservationId, cascadeDelete: true)
                .ForeignKey("Variable_Definition_TBL", t => t.QualifierId, cascadeDelete: true)
                .Index(t => t.ObservationId)
                .Index(t => t.QualifierId);
            
            CreateTable(
                "Observation_Synonyms",
                c => new
                    {
                        ObservationId = c.Int(nullable: false),
                        QualifierId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ObservationId, t.QualifierId })
                .ForeignKey("Observation_TBL", t => t.ObservationId, cascadeDelete: true)
                .ForeignKey("Variable_Definition_TBL", t => t.QualifierId, cascadeDelete: true)
                .Index(t => t.ObservationId)
                .Index(t => t.QualifierId);
            
            CreateTable(
                "Observation_Timings",
                c => new
                    {
                        ObservationId = c.Int(nullable: false),
                        QualifierId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ObservationId, t.QualifierId })
                .ForeignKey("Observation_TBL", t => t.ObservationId, cascadeDelete: true)
                .ForeignKey("Variable_Definition_TBL", t => t.QualifierId, cascadeDelete: true)
                .Index(t => t.ObservationId)
                .Index(t => t.QualifierId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Observation_TBL", "TopicVariableId", "Variable_Definition_TBL");
            DropForeignKey("Observation_Timings", "QualifierId", "Variable_Definition_TBL");
            DropForeignKey("Observation_Timings", "ObservationId", "Observation_TBL");
            DropForeignKey("Observation_Synonyms", "QualifierId", "Variable_Definition_TBL");
            DropForeignKey("Observation_Synonyms", "ObservationId", "Observation_TBL");
            DropForeignKey("Observation_Qualfiers", "QualifierId", "Variable_Definition_TBL");
            DropForeignKey("Observation_Qualfiers", "ObservationId", "Observation_TBL");
            DropForeignKey("Observation_TBL", "DefaultQualifierId", "Variable_Definition_TBL");
            DropForeignKey("Observation_TBL", "ControlledTermId", "CVterm_TBL");
            DropForeignKey("Variable_Qualifiers", "QualifierId", "Variable_Definition_TBL");
            DropForeignKey("Variable_Qualifiers", "VariableId", "Variable_Definition_TBL");
            DropIndex("Observation_Timings", new[] { "QualifierId" });
            DropIndex("Observation_Timings", new[] { "ObservationId" });
            DropIndex("Observation_Synonyms", new[] { "QualifierId" });
            DropIndex("Observation_Synonyms", new[] { "ObservationId" });
            DropIndex("Observation_Qualfiers", new[] { "QualifierId" });
            DropIndex("Observation_Qualfiers", new[] { "ObservationId" });
            DropIndex("Variable_Qualifiers", new[] { "QualifierId" });
            DropIndex("Variable_Qualifiers", new[] { "VariableId" });
            DropIndex("Observation_TBL", new[] { "DefaultQualifierId" });
            DropIndex("Observation_TBL", new[] { "TopicVariableId" });
            DropIndex("Observation_TBL", new[] { "ControlledTermId" });
            DropTable("Observation_Timings");
            DropTable("Observation_Synonyms");
            DropTable("Observation_Qualfiers");
            DropTable("Variable_Qualifiers");
            DropTable("Observation_TBL");
        }
    }
}
