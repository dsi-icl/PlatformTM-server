namespace eTRIKS.Commons.Persistence.Migrations_prod
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddatafiles : DbMigration
    {
        public override void Up()
        {

            CreateTable(
                "DataFiles_TBL",
                c => new
                    {
                        DataFileId = c.Int(nullable: false, identity: true),
                        FileName = c.String(unicode: false),
                        DataType = c.String(unicode: false),
                        DateAdded = c.String(unicode: false),
                        LastModified = c.String(unicode: false),
                        State = c.String(unicode: false),
                        ProjectId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DataFileId)
                .ForeignKey("Project_TBL", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "Dataset_DataFiles",
                c => new
                    {
                        DataFileId = c.Int(nullable: false),
                        DatasetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DataFileId, t.DatasetId })
                .ForeignKey("DataFiles_TBL", t => t.DataFileId, cascadeDelete: true)
                .ForeignKey("Dataset_TBL", t => t.DatasetId, cascadeDelete: true)
                .Index(t => t.DataFileId)
                .Index(t => t.DatasetId);
           
        }
        
        public override void Down()
        {
            //AddColumn("dbo.AspNetUsers", "Discriminator", c => c.String(nullable: false, maxLength: 128, storeType: "nvarchar"));
            //DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("DataFiles_TBL", "ProjectId", "dbo.Project_TBL");
            DropForeignKey("Dataset_DataFiles", "DatasetId", "dbo.Dataset_TBL");
            DropForeignKey("Dataset_DataFiles", "DataFileId", "dbo.DataFiles_TBL");
            DropIndex("Dataset_DataFiles", new[] { "DatasetId" });
            DropIndex("Dataset_DataFiles", new[] { "DataFileId" });
            DropIndex("DataFiles_TBL", new[] { "ProjectId" });
            //DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            //DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            //DropIndex("dbo.AspNetUsers", "UserNameIndex");
            //DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            //DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            //DropPrimaryKey("dbo.AspNetUserLogins");
            //DropPrimaryKey("dbo.AspNetUserRoles");
            //AlterColumn("dbo.AspNetUserLogins", "UserId", c => c.String(maxLength: 128, storeType: "nvarchar"));
            //AlterColumn("dbo.AspNetUserClaims", "UserId", c => c.String(maxLength: 128, storeType: "nvarchar"));
            //AlterColumn("dbo.AspNetUserClaims", "UserId", c => c.String(unicode: false));
            //AlterColumn("dbo.AspNetUsers", "JoinDate", c => c.DateTime(precision: 0));
            //AlterColumn("dbo.AspNetUsers", "Level", c => c.Byte());
            //AlterColumn("dbo.AspNetUsers", "LastName", c => c.String(maxLength: 100, storeType: "nvarchar"));
            //AlterColumn("dbo.AspNetUsers", "FirstName", c => c.String(maxLength: 100, storeType: "nvarchar"));
            //AlterColumn("dbo.AspNetUsers", "UserName", c => c.String(nullable: false, maxLength: 128, storeType: "nvarchar"));
            //AlterColumn("dbo.AspNetUsers", "Email", c => c.String(unicode: false));
            //AlterColumn("dbo.AspNetUserRoles", "UserId", c => c.String(maxLength: 128, storeType: "nvarchar"));
            //AlterColumn("dbo.AspNetRoles", "Name", c => c.String(nullable: false, maxLength: 128, storeType: "nvarchar"));
            DropTable("Dataset_DataFiles");
            DropTable("DataFiles_TBL");
            //AddPrimaryKey("dbo.AspNetUserLogins", new[] { "LoginProvider", "ProviderKey", "UserId" });
            //AddPrimaryKey("dbo.AspNetUserRoles", new[] { "UserId", "RoleId" });
            //RenameColumn(table: "dbo.AspNetUserRoles", name: "UserId", newName: "IdentityUser_Id");
            //RenameColumn(table: "dbo.AspNetUserLogins", name: "UserId", newName: "IdentityUser_Id");
            //RenameColumn(table: "dbo.AspNetUserClaims", name: "UserId", newName: "IdentityUser_Id");
            //AddColumn("dbo.AspNetUserLogins", "UserId", c => c.String(nullable: false, maxLength: 128, storeType: "nvarchar"));
            //AddColumn("dbo.AspNetUserClaims", "UserId", c => c.String(unicode: false));
            //AddColumn("dbo.AspNetUserRoles", "UserId", c => c.String(nullable: false, maxLength: 128, storeType: "nvarchar"));
            //CreateIndex("dbo.AspNetUserLogins", "IdentityUser_Id");
            //CreateIndex("dbo.AspNetUserClaims", "IdentityUser_Id");
            //CreateIndex("dbo.AspNetUserRoles", "IdentityUser_Id");
            //CreateIndex("dbo.AspNetRoles", "Name", unique: true, name: "RoleNameIndex");
            //AddForeignKey("dbo.AspNetUserRoles", "IdentityUser_Id", "dbo.AspNetUsers", "Id");
            //AddForeignKey("dbo.AspNetUserLogins", "IdentityUser_Id", "dbo.AspNetUsers", "Id");
            //AddForeignKey("dbo.AspNetUserClaims", "IdentityUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
