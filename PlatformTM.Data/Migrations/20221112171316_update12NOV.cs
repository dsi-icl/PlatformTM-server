using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformTM.Data.Migrations
{
    public partial class update12NOV : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Acronym",
                table: "PrimaryDataset",
                type: "varchar(10)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Acronym",
                table: "PrimaryDataset");
        }
    }
}
