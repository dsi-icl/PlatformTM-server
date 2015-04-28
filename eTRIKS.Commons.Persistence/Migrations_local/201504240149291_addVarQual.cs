namespace eTRIKS.Commons.Persistence.Migrations_local
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addVarQual : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("Variable_Qualifiers", "QualifierId", "Variable_Definition_TBL");
            DropForeignKey("Variable_Qualifiers", "VariableId", "Variable_Definition_TBL");
            DropIndex("Variable_Qualifiers", new[] { "QualifierId" });
            DropIndex("Variable_Qualifiers", new[] { "VariableId" });
            DropTable("Variable_Qualifiers");
        }
    }
}
