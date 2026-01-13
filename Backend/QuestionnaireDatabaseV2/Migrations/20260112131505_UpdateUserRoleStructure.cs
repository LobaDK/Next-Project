using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuestionnaireDatabaseV2.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserRoleStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaires_Questionnaires_CopiedFromQuestionnaireId",
                table: "Questionnaires");

            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaires_Users_CreatedByUserId",
                table: "Questionnaires");

            migrationBuilder.DropTable(
                name: "AssignmentViewers");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Responses_AssignmentId_ParticipantId_SubmittedAt",
                table: "Responses");

            migrationBuilder.DropIndex(
                name: "IX_Responses_IsCompleted",
                table: "Responses");

            migrationBuilder.DropIndex(
                name: "IX_Questionnaires_CopiedFromQuestionnaireId",
                table: "Questionnaires");

            migrationBuilder.DropIndex(
                name: "IX_AssignmentParticipants_HasResponded",
                table: "AssignmentParticipants");

            migrationBuilder.DropIndex(
                name: "IX_AssignmentParticipants_IsRequired",
                table: "AssignmentParticipants");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastLoginAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AnswersHash",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "CompletionTimeMinutes",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "IsFinalResponse",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "QuestionnaireSchemaHashAtSubmission",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "SequenceNumber",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "SubmissionIPAddress",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "SchemaHash",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "AllowMultipleResponses",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "Configuration",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "MaxResponses",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "FirstResponseAt",
                table: "AssignmentParticipants");

            migrationBuilder.DropColumn(
                name: "HasResponded",
                table: "AssignmentParticipants");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "AssignmentParticipants");

            migrationBuilder.DropColumn(
                name: "LastResponseAt",
                table: "AssignmentParticipants");

            migrationBuilder.DropColumn(
                name: "NotifiedAt",
                table: "AssignmentParticipants");

            migrationBuilder.DropColumn(
                name: "ResponseCount",
                table: "AssignmentParticipants");

            migrationBuilder.DropColumn(
                name: "RoleInAssignment",
                table: "AssignmentParticipants");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Users",
                newName: "IsManager");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "Users",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Questionnaires",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsAnonymous",
                table: "Assignments",
                newName: "AllowAnonymizedManagerViewing");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserRoles",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Questionnaires",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Version_CopiedFromQuestionnaireId",
                table: "Questionnaires",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Permissions",
                table: "AssignmentParticipants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_AssignmentId_ParticipantId",
                table: "Responses",
                columns: new[] { "AssignmentId", "ParticipantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_UserId",
                table: "Questionnaires",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_Version_CopiedFromQuestionnaireId",
                table: "Questionnaires",
                column: "Version_CopiedFromQuestionnaireId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questionnaires_Questionnaires_Version_CopiedFromQuestionnaireId",
                table: "Questionnaires",
                column: "Version_CopiedFromQuestionnaireId",
                principalTable: "Questionnaires",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Questionnaires_Users_UserId",
                table: "Questionnaires",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaires_Questionnaires_Version_CopiedFromQuestionnaireId",
                table: "Questionnaires");

            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaires_Users_UserId",
                table: "Questionnaires");

            migrationBuilder.DropIndex(
                name: "IX_Responses_AssignmentId_ParticipantId",
                table: "Responses");

            migrationBuilder.DropIndex(
                name: "IX_Questionnaires_UserId",
                table: "Questionnaires");

            migrationBuilder.DropIndex(
                name: "IX_Questionnaires_Version_CopiedFromQuestionnaireId",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserRoles",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "Version_CopiedFromQuestionnaireId",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "Permissions",
                table: "AssignmentParticipants");

            migrationBuilder.RenameColumn(
                name: "IsManager",
                table: "Users",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Users",
                newName: "DisplayName");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Questionnaires",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "AllowAnonymizedManagerViewing",
                table: "Assignments",
                newName: "IsAnonymous");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswersHash",
                table: "Responses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompletionTimeMinutes",
                table: "Responses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinalResponse",
                table: "Responses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Responses",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "Responses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestionnaireSchemaHashAtSubmission",
                table: "Responses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SequenceNumber",
                table: "Responses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SubmissionIPAddress",
                table: "Responses",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "Responses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "Questionnaires",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchemaHash",
                table: "Questionnaires",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowMultipleResponses",
                table: "Assignments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Configuration",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Assignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxResponses",
                table: "Assignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Assignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstResponseAt",
                table: "AssignmentParticipants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasResponded",
                table: "AssignmentParticipants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "AssignmentParticipants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastResponseAt",
                table: "AssignmentParticipants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NotifiedAt",
                table: "AssignmentParticipants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResponseCount",
                table: "AssignmentParticipants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RoleInAssignment",
                table: "AssignmentParticipants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActiveDirectoryGroupDN = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsSystemRole = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentViewers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrantedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CanExportData = table.Column<bool>(type: "bit", nullable: false),
                    CanViewParticipantIdentities = table.Column<bool>(type: "bit", nullable: false),
                    CanViewRawResponses = table.Column<bool>(type: "bit", nullable: false),
                    CanViewSummary = table.Column<bool>(type: "bit", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GrantedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ViewingConfiguration = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentViewers", x => x.Id);
                    table.CheckConstraint("CK_AssignmentViewer_UserOrRole", "(UserId IS NOT NULL AND RoleId IS NULL) OR (UserId IS NULL AND RoleId IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_AssignmentViewers_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignmentViewers_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssignmentViewers_Users_GrantedByUserId",
                        column: x => x.GrantedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssignmentViewers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsFromActiveDirectory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_AssignedByUserId",
                        column: x => x.AssignedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ActiveDirectoryGroupDN", "CreatedAt", "CreatedByUserId", "Description", "IsActive", "IsSystemRole", "LastUpdatedAt", "Name" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "System administrator with full access", true, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin" },
                    { 2, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Can create questionnaires and assignments, typically sees summary results only", true, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manager" },
                    { 3, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Can view assigned questionnaire results, often sees full raw data", true, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Teacher" },
                    { 4, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Participates in questionnaires, can view own responses", true, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Student" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Responses_AssignmentId_ParticipantId_SubmittedAt",
                table: "Responses",
                columns: new[] { "AssignmentId", "ParticipantId", "SubmittedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Responses_IsCompleted",
                table: "Responses",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_CopiedFromQuestionnaireId",
                table: "Questionnaires",
                column: "CopiedFromQuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentParticipants_HasResponded",
                table: "AssignmentParticipants",
                column: "HasResponded");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentParticipants_IsRequired",
                table: "AssignmentParticipants",
                column: "IsRequired");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentViewers_AssignmentId",
                table: "AssignmentViewers",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentViewers_GrantedByUserId",
                table: "AssignmentViewers",
                column: "GrantedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentViewers_RoleId",
                table: "AssignmentViewers",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentViewers_UserId",
                table: "AssignmentViewers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_ActiveDirectoryGroupDN",
                table: "Roles",
                column: "ActiveDirectoryGroupDN",
                unique: true,
                filter: "[ActiveDirectoryGroupDN] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CreatedByUserId",
                table: "Roles",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_AssignedByUserId",
                table: "UserRoles",
                column: "AssignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId_RoleId",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Questionnaires_Questionnaires_CopiedFromQuestionnaireId",
                table: "Questionnaires",
                column: "CopiedFromQuestionnaireId",
                principalTable: "Questionnaires",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Questionnaires_Users_CreatedByUserId",
                table: "Questionnaires",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
