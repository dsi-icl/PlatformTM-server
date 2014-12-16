namespace eTRIKS.Commons.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class firstMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Templates.DomainTemplate_TBL",
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
                "Templates.DomainVariable_TBL",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        Name = c.String(maxLength: 2000, storeType: "nvarchar"),
                        Description = c.String(maxLength: 2000, storeType: "nvarchar"),
                        DataType = c.String(maxLength: 200, storeType: "nvarchar"),
                        Label = c.String(maxLength: 2000, storeType: "nvarchar"),
                        DomainId = c.String(maxLength: 200, storeType: "nvarchar"),
                        VariableTypeId = c.String(maxLength: 200, storeType: "nvarchar"),
                        RoleTermId = c.String(maxLength: 200, storeType: "nvarchar"),
                        UsageTermId = c.String(maxLength: 200, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID)
                .ForeignKey("Templates.DomainTemplate_TBL", t => t.DomainId)
                .ForeignKey("CVterm_TBL", t => t.RoleTermId)
                .ForeignKey("CVterm_TBL", t => t.UsageTermId)
                .ForeignKey("CVterm_TBL", t => t.VariableTypeId)
                .Index(t => t.DomainId)
                .Index(t => t.VariableTypeId)
                .Index(t => t.RoleTermId)
                .Index(t => t.UsageTermId);
            
            CreateTable(
                "CVterm_TBL",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        Code = c.String(maxLength: 200, storeType: "nvarchar"),
                        Name = c.String(maxLength: 2000, storeType: "nvarchar"),
                        Definition = c.String(maxLength: 2000, storeType: "nvarchar"),
                        IsUserSpecified = c.Boolean(),
                        DictionartyId = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        XrefId = c.String(maxLength: 200, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID)
                .ForeignKey("Dictionary_TBL", t => t.DictionartyId, cascadeDelete: true)
                .ForeignKey("DBxref_TBL", t => t.XrefId)
                .Index(t => t.DictionartyId)
                .Index(t => t.XrefId);
            
            CreateTable(
                "Dictionary_TBL",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        Name = c.String(maxLength: 2000, storeType: "nvarchar"),
                        Definition = c.String(maxLength: 2000, storeType: "nvarchar"),
                        XrefId = c.String(maxLength: 200, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID)
                .ForeignKey("DBxref_TBL", t => t.XrefId)
                .Index(t => t.XrefId);
            
            CreateTable(
                "DBxref_TBL",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        Accession = c.String(maxLength: 200, storeType: "nvarchar"),
                        Description = c.String(maxLength: 2000, storeType: "nvarchar"),
                        DBId = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID)
                .ForeignKey("Db_TBL", t => t.DBId, cascadeDelete: true)
                .Index(t => t.DBId);
            
            CreateTable(
                "Db_TBL",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        Name = c.String(maxLength: 200, storeType: "nvarchar"),
                        Version = c.String(maxLength: 2000, storeType: "nvarchar"),
                        URL = c.String(maxLength: 2000, storeType: "nvarchar"),
                        URLPrefix = c.String(maxLength: 2000, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID);
            
            CreateTable(
                "Dataset_TBL",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        DataFile = c.String(maxLength: 2000, storeType: "nvarchar"),
                        ActivityId = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        DomainId = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID)
                .ForeignKey("Activity_TBL", t => t.ActivityId, cascadeDelete: true)
                .ForeignKey("Templates.DomainTemplate_TBL", t => t.DomainId, cascadeDelete: true)
                .Index(t => t.ActivityId)
                .Index(t => t.DomainId);
            
            CreateTable(
                "Activity_TBL",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        Name = c.String(maxLength: 2000, storeType: "nvarchar"),
                        StudyId = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID)
                .ForeignKey("Study_TBL", t => t.StudyId, cascadeDelete: true)
                .Index(t => t.StudyId);
            
            CreateTable(
                "Study_TBL",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        Name = c.String(maxLength: 2000, storeType: "nvarchar"),
                        Description = c.String(maxLength: 2000, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID);
            
            CreateTable(
                "Variable_Reference_TBL",
                c => new
                    {
                        VariableId = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        ActivityDatasetId = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        OrderNumber = c.Int(),
                        IsRequired = c.Boolean(),
                        KeySequence = c.Int(),
                    })
                .PrimaryKey(t => new { t.VariableId, t.ActivityDatasetId })
                .ForeignKey("Dataset_TBL", t => t.ActivityDatasetId, cascadeDelete: true)
                .ForeignKey("Variable_Definition_TBL", t => t.VariableId, cascadeDelete: true)
                .Index(t => t.VariableId)
                .Index(t => t.ActivityDatasetId);
            
            CreateTable(
                "Variable_Definition_TBL",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        Name = c.String(maxLength: 2000, storeType: "nvarchar"),
                        Label = c.String(unicode: false),
                        Description = c.String(maxLength: 2000, storeType: "nvarchar"),
                        DataType = c.String(maxLength: 200, storeType: "nvarchar"),
                        IsCurated = c.Boolean(),
                        VariableTypeId = c.String(maxLength: 200, storeType: "nvarchar"),
                        RoleId = c.String(maxLength: 200, storeType: "nvarchar"),
                        StudyId = c.String(maxLength: 200, storeType: "nvarchar"),
                        DerivedMethod_OID = c.String(maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID)
                .ForeignKey("CVterm_TBL", t => t.RoleId)
                .ForeignKey("Study_TBL", t => t.StudyId)
                .ForeignKey("CVterm_TBL", t => t.VariableTypeId)
                .ForeignKey("Derived_Method_TBL", t => t.DerivedMethod_OID)
                .Index(t => t.VariableTypeId)
                .Index(t => t.RoleId)
                .Index(t => t.StudyId)
                .Index(t => t.DerivedMethod_OID);
            
            CreateTable(
                "Derived_Method_TBL",
                c => new
                    {
                        OID = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        MethodName = c.String(maxLength: 2000, storeType: "nvarchar"),
                        methodDescription = c.String(maxLength: 2000, storeType: "nvarchar"),
                        FormalExpression = c.String(unicode: false),
                        derivedVariableId = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        type = c.String(maxLength: 200, storeType: "nvarchar"),
                        DerivedValueType_OID = c.String(maxLength: 200, storeType: "nvarchar"),
                        DerivedVariable_OID = c.String(maxLength: 200, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.OID)
                .ForeignKey("CVterm_TBL", t => t.DerivedValueType_OID)
                .ForeignKey("Variable_Definition_TBL", t => t.DerivedVariable_OID)
                .Index(t => t.DerivedValueType_OID)
                .Index(t => t.DerivedVariable_OID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Variable_Definition_TBL", "DerivedMethod_OID", "Derived_Method_TBL");
            DropForeignKey("Derived_Method_TBL", "DerivedVariable_OID", "Variable_Definition_TBL");
            DropForeignKey("Derived_Method_TBL", "DerivedValueType_OID", "CVterm_TBL");
            DropForeignKey("Variable_Reference_TBL", "VariableId", "Variable_Definition_TBL");
            DropForeignKey("Variable_Definition_TBL", "VariableTypeId", "CVterm_TBL");
            DropForeignKey("Variable_Definition_TBL", "StudyId", "Study_TBL");
            DropForeignKey("Variable_Definition_TBL", "RoleId", "CVterm_TBL");
            DropForeignKey("Variable_Reference_TBL", "ActivityDatasetId", "Dataset_TBL");
            DropForeignKey("Dataset_TBL", "DomainId", "Templates.DomainTemplate_TBL");
            DropForeignKey("Dataset_TBL", "ActivityId", "Activity_TBL");
            DropForeignKey("Activity_TBL", "StudyId", "Study_TBL");
            DropForeignKey("Templates.DomainVariable_TBL", "VariableTypeId", "CVterm_TBL");
            DropForeignKey("Templates.DomainVariable_TBL", "UsageTermId", "CVterm_TBL");
            DropForeignKey("Templates.DomainVariable_TBL", "RoleTermId", "CVterm_TBL");
            DropForeignKey("CVterm_TBL", "XrefId", "DBxref_TBL");
            DropForeignKey("CVterm_TBL", "DictionartyId", "Dictionary_TBL");
            DropForeignKey("Dictionary_TBL", "XrefId", "DBxref_TBL");
            DropForeignKey("DBxref_TBL", "DBId", "Db_TBL");
            DropForeignKey("Templates.DomainVariable_TBL", "DomainId", "Templates.DomainTemplate_TBL");
            DropIndex("Derived_Method_TBL", new[] { "DerivedVariable_OID" });
            DropIndex("Derived_Method_TBL", new[] { "DerivedValueType_OID" });
            DropIndex("Variable_Definition_TBL", new[] { "DerivedMethod_OID" });
            DropIndex("Variable_Definition_TBL", new[] { "StudyId" });
            DropIndex("Variable_Definition_TBL", new[] { "RoleId" });
            DropIndex("Variable_Definition_TBL", new[] { "VariableTypeId" });
            DropIndex("Variable_Reference_TBL", new[] { "ActivityDatasetId" });
            DropIndex("Variable_Reference_TBL", new[] { "VariableId" });
            DropIndex("Activity_TBL", new[] { "StudyId" });
            DropIndex("Dataset_TBL", new[] { "DomainId" });
            DropIndex("Dataset_TBL", new[] { "ActivityId" });
            DropIndex("DBxref_TBL", new[] { "DBId" });
            DropIndex("Dictionary_TBL", new[] { "XrefId" });
            DropIndex("CVterm_TBL", new[] { "XrefId" });
            DropIndex("CVterm_TBL", new[] { "DictionartyId" });
            DropIndex("Templates.DomainVariable_TBL", new[] { "UsageTermId" });
            DropIndex("Templates.DomainVariable_TBL", new[] { "RoleTermId" });
            DropIndex("Templates.DomainVariable_TBL", new[] { "VariableTypeId" });
            DropIndex("Templates.DomainVariable_TBL", new[] { "DomainId" });
            DropTable("Derived_Method_TBL");
            DropTable("Variable_Definition_TBL");
            DropTable("Variable_Reference_TBL");
            DropTable("Study_TBL");
            DropTable("Activity_TBL");
            DropTable("Dataset_TBL");
            DropTable("Db_TBL");
            DropTable("DBxref_TBL");
            DropTable("Dictionary_TBL");
            DropTable("CVterm_TBL");
            DropTable("Templates.DomainVariable_TBL");
            DropTable("Templates.DomainTemplate_TBL");
        }
    }
}
