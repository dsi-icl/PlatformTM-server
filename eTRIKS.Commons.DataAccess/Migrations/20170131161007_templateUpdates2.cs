using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace eTRIKS.Commons.DataAccess.Migrations
{
    public partial class templateUpdates2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllowableQualifiersId",
                table: "DomainTemplateVariables",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowMultipleValues",
                table: "VariableDefinitions",
                nullable: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsGeneric",
                table: "VariableDefinitions",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "NameQualifier",
                table: "VariableDefinitions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Section",
                table: "VariableDefinitions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DomainTemplateVariables_AllowableQualifiersId",
                table: "DomainTemplateVariables",
                column: "AllowableQualifiersId");

            migrationBuilder.AddForeignKey(
                name: "FK_DomainTemplateVariables_Dictionaries",
                table: "DomainTemplateVariables",
                column: "AllowableQualifiersId",
                principalTable: "Dictionaries",
                principalColumn: "OID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DomainTemplateVariables_Dictionaries_AllowableQualifiersId",
                table: "DomainTemplateVariables");

            migrationBuilder.DropIndex(
                name: "IX_DomainTemplateVariables_AllowableQualifiersId",
                table: "DomainTemplateVariables");

            migrationBuilder.DropColumn(
                name: "AllowableQualifiersId",
                table: "DomainTemplateVariables");

            migrationBuilder.DropColumn(
                name: "AllowMultipleValues",
                table: "VariableDefinitions");

            migrationBuilder.DropColumn(
                name: "IsGeneric",
                table: "VariableDefinitions");

            migrationBuilder.DropColumn(
                name: "NameQualifier",
                table: "VariableDefinitions");

            migrationBuilder.DropColumn(
                name: "Section",
                table: "VariableDefinitions");
        }
    }
}
