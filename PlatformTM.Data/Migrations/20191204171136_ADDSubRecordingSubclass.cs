using Microsoft.EntityFrameworkCore.Migrations;

namespace PlatformTM.Data.Migrations
{
    public partial class ADDSubRecordingSubclass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<int>(
            //    name: "ParentId",
            //    table: "DataFiles",
            //    nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubjectType",
                table: "Activities",
                nullable: true);

            //migrationBuilder.CreateTable(
            //    name: "Variable_Qualifiers",
            //    columns: table => new
            //    {
            //        VariableId = table.Column<int>(nullable: false),
            //        QualifierId = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Variable_Qualifiers", x => new { x.QualifierId, x.VariableId });
            //        table.ForeignKey(
            //            name: "FK_Variable_Qualifiers_VariableDefinitions_QualifierId",
            //            column: x => x.QualifierId,
            //            principalTable: "VariableDefinitions",
            //            principalColumn: "OID",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Variable_Qualifiers_VariableDefinitions_VariableId",
            //            column: x => x.VariableId,
            //            principalTable: "VariableDefinitions",
            //            principalColumn: "OID",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_DataFiles_ParentId",
            //    table: "DataFiles",
            //    column: "ParentId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Variable_Qualifiers_VariableId",
            //    table: "Variable_Qualifiers",
            //    column: "VariableId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_DataFiles_DataFiles_ParentId",
            //    table: "DataFiles",
            //    column: "ParentId",
            //    principalTable: "DataFiles",
            //    principalColumn: "DataFileId",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataFiles_DataFiles_ParentId",
                table: "DataFiles");

            migrationBuilder.DropTable(
                name: "Variable_Qualifiers");

            migrationBuilder.DropIndex(
                name: "IX_DataFiles_ParentId",
                table: "DataFiles");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "DataFiles");

            migrationBuilder.DropColumn(
                name: "SubjectType",
                table: "Activities");
        }
    }
}
