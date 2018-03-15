using Microsoft.EntityFrameworkCore.Migrations;

namespace PlatformTM.Data.Migrations
{
    public partial class sampleMods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_DomainTemplateVariables_Dictionaries_AllowableQualifiersId",
            //    table: "DomainTemplateVariables");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_DomainTemplateVariables_Dictionaries_DictionaryId",
            //    table: "DomainTemplateVariables");

            //migrationBuilder.DropColumn(
            //    name: "DatasetDomainCode",
            //    table: "Characteristics");

            //migrationBuilder.RenameColumn(
            //    name: "DictionaryId",
            //    table: "DomainTemplateVariables",
            //    newName: "CVTermsDictionaryId");

            //migrationBuilder.RenameColumn(
            //    name: "AllowableQualifiersId",
            //    table: "DomainTemplateVariables",
            //    newName: "QualifiersDictionaryId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_DomainTemplateVariables_DictionaryId",
            //    table: "DomainTemplateVariables",
            //    newName: "IX_DomainTemplateVariables_CVTermsDictionaryId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_DomainTemplateVariables_AllowableQualifiersId",
            //    table: "DomainTemplateVariables",
            //    newName: "IX_DomainTemplateVariables_QualifiersDictionaryId");

            //migrationBuilder.AddColumn<int>(
            //    name: "ActivityId",
            //    table: "CharacteristicObjects",
            //    nullable: true);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "CollectionDateTime",
            //    table: "BioSamples",
            //    nullable: false,
            //    defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            //migrationBuilder.AddColumn<int>(
            //    name: "CollectionStudyDayId",
            //    table: "BioSamples",
            //    nullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "CollectionStudyTimePointId",
            //    table: "BioSamples",
            //    nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasTemporalData",
                table: "Activities",
                nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_CharacteristicObjects_ActivityId",
            //    table: "CharacteristicObjects",
            //    column: "ActivityId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_BioSamples_CollectionStudyDayId",
            //    table: "BioSamples",
            //    column: "CollectionStudyDayId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_BioSamples_CollectionStudyTimePointId",
            //    table: "BioSamples",
            //    column: "CollectionStudyTimePointId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_BioSamples_Timepoints_CollectionStudyDayId",
            //    table: "BioSamples",
            //    column: "CollectionStudyDayId",
            //    principalTable: "Timepoints",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_BioSamples_Timepoints_CollectionStudyTimePointId",
            //    table: "BioSamples",
            //    column: "CollectionStudyTimePointId",
            //    principalTable: "Timepoints",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_CharacteristicObjects_Activities_ActivityId",
            //    table: "CharacteristicObjects",
            //    column: "ActivityId",
            //    principalTable: "Activities",
            //    principalColumn: "ActivityId",
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_DomainTemplateVariables_Dictionaries_CVTermsDictionaryId",
            //    table: "DomainTemplateVariables",
            //    column: "CVTermsDictionaryId",
            //    principalTable: "Dictionaries",
            //    principalColumn: "OID",
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_DomainTemplateVariables_Dictionaries_QualifiersDictionaryId",
            //    table: "DomainTemplateVariables",
            //    column: "QualifiersDictionaryId",
            //    principalTable: "Dictionaries",
            //    principalColumn: "OID",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BioSamples_Timepoints_CollectionStudyDayId",
                table: "BioSamples");

            migrationBuilder.DropForeignKey(
                name: "FK_BioSamples_Timepoints_CollectionStudyTimePointId",
                table: "BioSamples");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacteristicObjects_Activities_ActivityId",
                table: "CharacteristicObjects");

            migrationBuilder.DropForeignKey(
                name: "FK_DomainTemplateVariables_Dictionaries_CVTermsDictionaryId",
                table: "DomainTemplateVariables");

            migrationBuilder.DropForeignKey(
                name: "FK_DomainTemplateVariables_Dictionaries_QualifiersDictionaryId",
                table: "DomainTemplateVariables");

            migrationBuilder.DropIndex(
                name: "IX_CharacteristicObjects_ActivityId",
                table: "CharacteristicObjects");

            migrationBuilder.DropIndex(
                name: "IX_BioSamples_CollectionStudyDayId",
                table: "BioSamples");

            migrationBuilder.DropIndex(
                name: "IX_BioSamples_CollectionStudyTimePointId",
                table: "BioSamples");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "CharacteristicObjects");

            migrationBuilder.DropColumn(
                name: "CollectionDateTime",
                table: "BioSamples");

            migrationBuilder.DropColumn(
                name: "CollectionStudyDayId",
                table: "BioSamples");

            migrationBuilder.DropColumn(
                name: "CollectionStudyTimePointId",
                table: "BioSamples");

            migrationBuilder.DropColumn(
                name: "HasTemporalData",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "CVTermsDictionaryId",
                table: "DomainTemplateVariables",
                newName: "DictionaryId");

            migrationBuilder.RenameColumn(
                name: "QualifiersDictionaryId",
                table: "DomainTemplateVariables",
                newName: "AllowableQualifiersId");

            migrationBuilder.RenameIndex(
                name: "IX_DomainTemplateVariables_QualifiersDictionaryId",
                table: "DomainTemplateVariables",
                newName: "IX_DomainTemplateVariables_AllowableQualifiersId");

            migrationBuilder.RenameIndex(
                name: "IX_DomainTemplateVariables_CVTermsDictionaryId",
                table: "DomainTemplateVariables",
                newName: "IX_DomainTemplateVariables_DictionaryId");

            migrationBuilder.AddColumn<string>(
                name: "DatasetDomainCode",
                table: "Characteristics",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DomainTemplateVariables_Dictionaries_AllowableQualifiersId",
                table: "DomainTemplateVariables",
                column: "AllowableQualifiersId",
                principalTable: "Dictionaries",
                principalColumn: "OID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DomainTemplateVariables_Dictionaries_DictionaryId",
                table: "DomainTemplateVariables",
                column: "DictionaryId",
                principalTable: "Dictionaries",
                principalColumn: "OID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
