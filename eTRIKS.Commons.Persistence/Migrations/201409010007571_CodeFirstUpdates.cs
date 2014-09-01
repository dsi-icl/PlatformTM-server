namespace eTRIKS.Commons.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CodeFirstUpdates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Activity_TAB",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        name = c.String(maxLength: 2000, storeType: "nvarchar"),
                        studyId = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID)
                .ForeignKey("Study_TAB", t => t.studyId, cascadeDelete: true)
                .Index(t => t.studyId);
            
            CreateTable(
                "Dataset_TAB",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        dataFile = c.String(maxLength: 2000, storeType: "nvarchar"),
                        activityId = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        domainId = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        Domain_OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID)
                .ForeignKey("Activity_TAB", t => t.activityId, cascadeDelete: true)
                .ForeignKey("Dataset_Template_TAB", t => t.Domain_OID, cascadeDelete: true)
                .Index(t => t.activityId)
                .Index(t => t.Domain_OID);
            
            CreateTable(
                "Dataset_Template_TAB",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        domainName = c.String(maxLength: 2000, storeType: "nvarchar"),
                        _class = c.String(name: "class", maxLength: 200, storeType: "nvarchar"),
                        description = c.String(maxLength: 2000, storeType: "nvarchar"),
                        code = c.String(maxLength: 200, storeType: "nvarchar"),
                        structure = c.String(maxLength: 200, storeType: "nvarchar"),
                        repeating = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.OID);
            
            CreateTable(
                "Variable_Template_TAB",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        Name = c.String(maxLength: 2000, storeType: "nvarchar"),
                        Description = c.String(maxLength: 2000, storeType: "nvarchar"),
                        DataType = c.String(maxLength: 200, storeType: "nvarchar"),
                        Label = c.String(maxLength: 2000, storeType: "nvarchar"),
                        DomainId = c.String(maxLength: 200, storeType: "nvarchar"),
                        Role = c.String(maxLength: 200, storeType: "nvarchar"),
                        Usage = c.String(maxLength: 200, storeType: "nvarchar"),
                        VariableType = c.String(maxLength: 200, storeType: "nvarchar"),
                        Role_Code = c.String(maxLength: 200, storeType: "nvarchar"),
                        Usage_Code = c.String(maxLength: 200, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID)
                .ForeignKey("Dataset_Template_TAB", t => t.DomainId)
                .ForeignKey("CVterm_TAB", t => t.Role_Code)
                .ForeignKey("CVterm_TAB", t => t.Usage_Code)
                .Index(t => t.DomainId)
                .Index(t => t.Role_Code)
                .Index(t => t.Usage_Code);
            
            CreateTable(
                "CVterm_TAB",
                c => new
                    {
                        abbrv = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        name = c.String(maxLength: 2000, storeType: "nvarchar"),
                        order = c.Int(),
                        rank = c.Int(),
                        userSpecified = c.Boolean(),
                        dictionartyId = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        dbref = c.String(maxLength: 200, storeType: "nvarchar"),
                        externalReference_OID = c.String(maxLength: 200, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.abbrv)
                .ForeignKey("Dictionary_TAB", t => t.dictionartyId, cascadeDelete: true)
                .ForeignKey("DBxref_TAB", t => t.externalReference_OID)
                .Index(t => t.dictionartyId)
                .Index(t => t.externalReference_OID);
            
            CreateTable(
                "Dictionary_TAB",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        name = c.String(maxLength: 2000, storeType: "nvarchar"),
                        version = c.String(maxLength: 200, storeType: "nvarchar"),
                        url = c.String(maxLength: 2000, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID);
            
            CreateTable(
                "DBxref_TAB",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        accession = c.String(maxLength: 200, storeType: "nvarchar"),
                        description = c.String(maxLength: 2000, storeType: "nvarchar"),
                        db = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        DB_OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID)
                .ForeignKey("Db_TAB", t => t.DB_OID, cascadeDelete: true)
                .Index(t => t.DB_OID);
            
            CreateTable(
                "Db_TAB",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        name = c.String(maxLength: 200, storeType: "nvarchar"),
                        urlPrefix = c.String(maxLength: 2000, storeType: "nvarchar"),
                        url = c.String(maxLength: 2000, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID);
            
            CreateTable(
                "Variable_Ref_TAB",
                c => new
                    {
                        variableId = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        activityDatasetId = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        orderNo = c.Int(),
                        mandatory = c.Boolean(),
                        keySequence = c.Int(),
                        Variable_OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.variableId, t.activityDatasetId })
                .ForeignKey("Dataset_TAB", t => t.activityDatasetId, cascadeDelete: true)
                .ForeignKey("Variable_Def_TAB", t => t.Variable_OID, cascadeDelete: true)
                .Index(t => t.activityDatasetId)
                .Index(t => t.Variable_OID);
            
            CreateTable(
                "Variable_Def_TAB",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        name = c.String(maxLength: 2000, storeType: "nvarchar"),
                        Label = c.String(unicode: false),
                        description = c.String(maxLength: 2000, storeType: "nvarchar"),
                        dataType = c.String(maxLength: 200, storeType: "nvarchar"),
                        isCurated = c.Boolean(),
                        variableType = c.String(maxLength: 200, storeType: "nvarchar"),
                        role = c.String(maxLength: 200, storeType: "nvarchar"),
                        studyId = c.String(maxLength: 200, storeType: "nvarchar"),
                        DerivedVariablePropertiesId = c.String(unicode: false),
                        DerivedMethod_OID = c.String(maxLength: 200, storeType: "nvarchar"),
                        Role_Code = c.String(maxLength: 200, storeType: "nvarchar"),
                        study_OID = c.String(maxLength: 200, storeType: "nvarchar"),
                        VariableType_Code = c.String(maxLength: 200, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID)
                .ForeignKey("Derived_Method_TAB", t => t.DerivedMethod_OID)
                .ForeignKey("CVterm_TAB", t => t.Role_Code)
                .ForeignKey("Study_TAB", t => t.study_OID)
                .ForeignKey("CVterm_TAB", t => t.VariableType_Code)
                .Index(t => t.DerivedMethod_OID)
                .Index(t => t.Role_Code)
                .Index(t => t.study_OID)
                .Index(t => t.VariableType_Code);
            
            CreateTable(
                "Derived_Method_TAB",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        MethodName = c.String(maxLength: 2000, storeType: "nvarchar"),
                        methodDescription = c.String(maxLength: 2000, storeType: "nvarchar"),
                        FormalExpression = c.String(unicode: false),
                        derivedVariableId = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        type = c.String(maxLength: 200, storeType: "nvarchar"),
                        DerivedValueType_Code = c.String(maxLength: 200, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID)
                .ForeignKey("CVterm_TAB", t => t.DerivedValueType_Code)
                .ForeignKey("Variable_Def_TAB", t => t.OID)
                .Index(t => t.OID)
                .Index(t => t.DerivedValueType_Code);
            
            CreateTable(
                "Study_TAB",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        name = c.String(maxLength: 2000, storeType: "nvarchar"),
                        description = c.String(maxLength: 2000, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Activity_TAB", "studyId", "Study_TAB");
            DropForeignKey("Variable_Ref_TAB", "Variable_OID", "Variable_Def_TAB");
            DropForeignKey("Variable_Def_TAB", "VariableType_Code", "CVterm_TAB");
            DropForeignKey("Variable_Def_TAB", "study_OID", "Study_TAB");
            DropForeignKey("Variable_Def_TAB", "Role_Code", "CVterm_TAB");
            DropForeignKey("Variable_Def_TAB", "DerivedMethod_OID", "Derived_Method_TAB");
            DropForeignKey("Derived_Method_TAB", "OID", "Variable_Def_TAB");
            DropForeignKey("Derived_Method_TAB", "DerivedValueType_Code", "CVterm_TAB");
            DropForeignKey("Variable_Ref_TAB", "activityDatasetId", "Dataset_TAB");
            DropForeignKey("Dataset_TAB", "Domain_OID", "Dataset_Template_TAB");
            DropForeignKey("Variable_Template_TAB", "Usage_Code", "CVterm_TAB");
            DropForeignKey("Variable_Template_TAB", "Role_Code", "CVterm_TAB");
            DropForeignKey("CVterm_TAB", "externalReference_OID", "DBxref_TAB");
            DropForeignKey("DBxref_TAB", "DB_OID", "Db_TAB");
            DropForeignKey("CVterm_TAB", "dictionartyId", "Dictionary_TAB");
            DropForeignKey("Variable_Template_TAB", "DomainId", "Dataset_Template_TAB");
            DropForeignKey("Dataset_TAB", "activityId", "Activity_TAB");
            DropIndex("Derived_Method_TAB", new[] { "DerivedValueType_Code" });
            DropIndex("Derived_Method_TAB", new[] { "OID" });
            DropIndex("Variable_Def_TAB", new[] { "VariableType_Code" });
            DropIndex("Variable_Def_TAB", new[] { "study_OID" });
            DropIndex("Variable_Def_TAB", new[] { "Role_Code" });
            DropIndex("Variable_Def_TAB", new[] { "DerivedMethod_OID" });
            DropIndex("Variable_Ref_TAB", new[] { "Variable_OID" });
            DropIndex("Variable_Ref_TAB", new[] { "activityDatasetId" });
            DropIndex("DBxref_TAB", new[] { "DB_OID" });
            DropIndex("CVterm_TAB", new[] { "externalReference_OID" });
            DropIndex("CVterm_TAB", new[] { "dictionartyId" });
            DropIndex("Variable_Template_TAB", new[] { "Usage_Code" });
            DropIndex("Variable_Template_TAB", new[] { "Role_Code" });
            DropIndex("Variable_Template_TAB", new[] { "DomainId" });
            DropIndex("Dataset_TAB", new[] { "Domain_OID" });
            DropIndex("Dataset_TAB", new[] { "activityId" });
            DropIndex("Activity_TAB", new[] { "studyId" });
            DropTable("Study_TAB");
            DropTable("Derived_Method_TAB");
            DropTable("Variable_Def_TAB");
            DropTable("Variable_Ref_TAB");
            DropTable("Db_TAB");
            DropTable("DBxref_TAB");
            DropTable("Dictionary_TAB");
            DropTable("CVterm_TAB");
            DropTable("Variable_Template_TAB");
            DropTable("Dataset_Template_TAB");
            DropTable("Dataset_TAB");
            DropTable("Activity_TAB");
        }
    }
}
