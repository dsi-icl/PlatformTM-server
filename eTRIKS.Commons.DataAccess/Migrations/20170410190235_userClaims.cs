using Microsoft.EntityFrameworkCore.Migrations;

namespace PlatformTM.Data.Migrations
{
    public partial class userClaims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserAccountClaims_UserAccounts_UserId",
            //    table: "UserAccountClaims");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_BioSamples_Timepoints_CollectionStudyDayId",
            //    table: "BioSamples");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_BioSamples_Timepoints_CollectionStudyTimePointId",
            //    table: "BioSamples");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Visits_Timepoints_StudyDayId",
            //    table: "Visits");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Timepoints_Visits_VisitId",
            //    table: "Timepoints");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Timepoints_Timepoints_ReferenceTimePointId",
            //    table: "Timepoints");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_Timepoints",
            //    table: "Timepoints");

            //migrationBuilder.DropIndex(
            //    name: "IX_Timepoints_VisitId",
            //    table: "Timepoints");

            //migrationBuilder.DropColumn(
            //    name: "VisitId",
            //    table: "Timepoints");

            //migrationBuilder.RenameTable(
            //    name: "Timepoints",
            //    newName: "TimePoints");

            //migrationBuilder.RenameIndex(
            //    name: "IX_Timepoints_ReferenceTimePointId",
            //    table: "TimePoints",
            //    newName: "IX_TimePoints_ReferenceTimePointId");

            //migrationBuilder.RenameColumn(
            //    name: "CharacterisitcId",
            //    table: "Characteristics",
            //    newName: "CharacteristicId");

            //migrationBuilder.RenameColumn(
            //    name: "UserId",
            //    table: "UserAccountClaims",
            //    newName: "UserAccountId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_UserAccountClaims_UserId",
            //    table: "UserAccountClaims",
            //    newName: "IX_UserAccountClaims_UserAccountId");

            //migrationBuilder.AddColumn<int>(
            //    name: "DatafileId",
            //    table: "Subjects",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_TimePoints",
            //    table: "TimePoints",
            //    column: "Id");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Subjects_DatafileId",
            //    table: "Subjects",
            //    column: "DatafileId",
            //    unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccountClaims_UserAccounts_UserAccountId",
                table: "UserAccountClaims",
                column: "UserAccountId",
                principalTable: "UserAccounts",
                principalColumn: "UserAccountId",
                onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_BioSamples_TimePoints_CollectionStudyDayId",
            //    table: "BioSamples",
            //    column: "CollectionStudyDayId",
            //    principalTable: "TimePoints",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_BioSamples_TimePoints_CollectionStudyTimePointId",
            //    table: "BioSamples",
            //    column: "CollectionStudyTimePointId",
            //    principalTable: "TimePoints",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Visits_TimePoints_StudyDayId",
            //    table: "Visits",
            //    column: "StudyDayId",
            //    principalTable: "TimePoints",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Subjects_DataFiles_DatafileId",
            //    table: "Subjects",
            //    column: "DatafileId",
            //    principalTable: "DataFiles",
            //    principalColumn: "DataFileId",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_TimePoints_TimePoints_ReferenceTimePointId",
            //    table: "TimePoints",
            //    column: "ReferenceTimePointId",
            //    principalTable: "TimePoints",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAccountClaims_UserAccounts_UserAccountId",
                table: "UserAccountClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_BioSamples_TimePoints_CollectionStudyDayId",
                table: "BioSamples");

            migrationBuilder.DropForeignKey(
                name: "FK_BioSamples_TimePoints_CollectionStudyTimePointId",
                table: "BioSamples");

            migrationBuilder.DropForeignKey(
                name: "FK_Visits_TimePoints_StudyDayId",
                table: "Visits");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_DataFiles_DatafileId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_TimePoints_TimePoints_ReferenceTimePointId",
                table: "TimePoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimePoints",
                table: "TimePoints");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_DatafileId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "DatafileId",
                table: "Subjects");

            migrationBuilder.RenameTable(
                name: "TimePoints",
                newName: "Timepoints");

            migrationBuilder.RenameIndex(
                name: "IX_TimePoints_ReferenceTimePointId",
                table: "Timepoints",
                newName: "IX_Timepoints_ReferenceTimePointId");

            migrationBuilder.RenameColumn(
                name: "CharacteristicId",
                table: "Characteristics",
                newName: "CharacterisitcId");

            migrationBuilder.RenameColumn(
                name: "UserAccountId",
                table: "UserAccountClaims",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAccountClaims_UserAccountId",
                table: "UserAccountClaims",
                newName: "IX_UserAccountClaims_UserId");

            migrationBuilder.AddColumn<int>(
                name: "VisitId",
                table: "Timepoints",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Timepoints",
                table: "Timepoints",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Timepoints_VisitId",
                table: "Timepoints",
                column: "VisitId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccountClaims_UserAccounts_UserId",
                table: "UserAccountClaims",
                column: "UserId",
                principalTable: "UserAccounts",
                principalColumn: "UserAccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BioSamples_Timepoints_CollectionStudyDayId",
                table: "BioSamples",
                column: "CollectionStudyDayId",
                principalTable: "Timepoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BioSamples_Timepoints_CollectionStudyTimePointId",
                table: "BioSamples",
                column: "CollectionStudyTimePointId",
                principalTable: "Timepoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Timepoints_StudyDayId",
                table: "Visits",
                column: "StudyDayId",
                principalTable: "Timepoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Timepoints_Visits_VisitId",
                table: "Timepoints",
                column: "VisitId",
                principalTable: "Visits",
                principalColumn: "VisitId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Timepoints_Timepoints_ReferenceTimePointId",
                table: "Timepoints",
                column: "ReferenceTimePointId",
                principalTable: "Timepoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
