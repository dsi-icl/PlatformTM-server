using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace eTRIKS.Commons.DataAccess.Migrations
{
    public partial class sampleMods2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DataFileId",
                table: "BioSamples",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BioSamples_DataFileId",
                table: "BioSamples",
                column: "DataFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_BioSamples_DataFiles_DataFileId",
                table: "BioSamples",
                column: "DataFileId",
                principalTable: "DataFiles",
                principalColumn: "DataFileId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BioSamples_DataFiles_DataFileId",
                table: "BioSamples");

            migrationBuilder.DropIndex(
                name: "IX_BioSamples_DataFileId",
                table: "BioSamples");

            migrationBuilder.DropColumn(
                name: "DataFileId",
                table: "BioSamples");
        }
    }
}
