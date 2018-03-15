using Microsoft.EntityFrameworkCore.Migrations;

namespace PlatformTM.Data.Migrations
{
    public partial class templateUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowMultipleValues",
                table: "DomainTemplateVariables",
                nullable: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGeneric",
                table: "DomainTemplateVariables",
                nullable: false
                );

            migrationBuilder.AddColumn<string>(
                name: "Section",
                table: "DomainTemplateVariables",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowMultipleValues",
                table: "DomainTemplateVariables");

            migrationBuilder.DropColumn(
                name: "IsGeneric",
                table: "DomainTemplateVariables");

            migrationBuilder.DropColumn(
                name: "Section",
                table: "DomainTemplateVariables");
        }
    }
}
