using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionnaireDatabaseV2.Migrations
{
    /// <inheritdoc />
    public partial class CopyBasedVersioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_QuestionnaireVersions_QuestionnaireVersionId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaires_QuestionnaireVersions_CurrentVersionId",
                table: "Questionnaires");

            migrationBuilder.DropForeignKey(
                name: "FK_Responses_QuestionnaireVersions_QuestionnaireVersionId",
                table: "Responses");

            migrationBuilder.DropTable(
                name: "QuestionnaireVersions");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_QuestionnaireVersionId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "QuestionnaireVersionId",
                table: "Assignments");

            migrationBuilder.RenameColumn(
                name: "QuestionnaireVersionId",
                table: "Responses",
                newName: "QuestionnaireId");

            migrationBuilder.RenameIndex(
                name: "IX_Responses_QuestionnaireVersionId",
                table: "Responses",
                newName: "IX_Responses_QuestionnaireId");

            migrationBuilder.RenameColumn(
                name: "CurrentVersionId",
                table: "Questionnaires",
                newName: "CopiedFromQuestionnaireId");

            migrationBuilder.RenameIndex(
                name: "IX_Questionnaires_CurrentVersionId",
                table: "Questionnaires",
                newName: "IX_Questionnaires_CopiedFromQuestionnaireId");

            migrationBuilder.AddColumn<string>(
                name: "QuestionnaireSchemaHashAtSubmission",
                table: "Responses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CopiedFromTitle",
                table: "Questionnaires",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchemaHash",
                table: "Questionnaires",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchemaJson",
                table: "Questionnaires",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Questionnaires_Questionnaires_CopiedFromQuestionnaireId",
                table: "Questionnaires",
                column: "CopiedFromQuestionnaireId",
                principalTable: "Questionnaires",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Responses_Questionnaires_QuestionnaireId",
                table: "Responses",
                column: "QuestionnaireId",
                principalTable: "Questionnaires",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaires_Questionnaires_CopiedFromQuestionnaireId",
                table: "Questionnaires");

            migrationBuilder.DropForeignKey(
                name: "FK_Responses_Questionnaires_QuestionnaireId",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "QuestionnaireSchemaHashAtSubmission",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "CopiedFromTitle",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "SchemaHash",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "SchemaJson",
                table: "Questionnaires");

            migrationBuilder.RenameColumn(
                name: "QuestionnaireId",
                table: "Responses",
                newName: "QuestionnaireVersionId");

            migrationBuilder.RenameIndex(
                name: "IX_Responses_QuestionnaireId",
                table: "Responses",
                newName: "IX_Responses_QuestionnaireVersionId");

            migrationBuilder.RenameColumn(
                name: "CopiedFromQuestionnaireId",
                table: "Questionnaires",
                newName: "CurrentVersionId");

            migrationBuilder.RenameIndex(
                name: "IX_Questionnaires_CopiedFromQuestionnaireId",
                table: "Questionnaires",
                newName: "IX_Questionnaires_CurrentVersionId");

            migrationBuilder.AddColumn<Guid>(
                name: "QuestionnaireVersionId",
                table: "Assignments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuestionnaireVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SchemaHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SchemaJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    VersionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireVersions_Questionnaires_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "Questionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionnaireVersions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_QuestionnaireVersionId",
                table: "Assignments",
                column: "QuestionnaireVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireVersions_CreatedByUserId",
                table: "QuestionnaireVersions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireVersions_IsActive",
                table: "QuestionnaireVersions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireVersions_QuestionnaireId_IsActive",
                table: "QuestionnaireVersions",
                columns: new[] { "QuestionnaireId", "IsActive" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireVersions_QuestionnaireId_Version",
                table: "QuestionnaireVersions",
                columns: new[] { "QuestionnaireId", "Version" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_QuestionnaireVersions_QuestionnaireVersionId",
                table: "Assignments",
                column: "QuestionnaireVersionId",
                principalTable: "QuestionnaireVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Questionnaires_QuestionnaireVersions_CurrentVersionId",
                table: "Questionnaires",
                column: "CurrentVersionId",
                principalTable: "QuestionnaireVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Responses_QuestionnaireVersions_QuestionnaireVersionId",
                table: "Responses",
                column: "QuestionnaireVersionId",
                principalTable: "QuestionnaireVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
