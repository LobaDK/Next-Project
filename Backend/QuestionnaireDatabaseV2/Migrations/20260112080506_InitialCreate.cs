using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuestionnaireDatabaseV2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActiveDirectoryGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ActiveDirectoryGroupDN = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsSystemRole = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
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
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    AssignedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsFromActiveDirectory = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "AssignmentParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleInAssignment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    HasResponded = table.Column<bool>(type: "bit", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    AddedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NotifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FirstResponseAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastResponseAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponseCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentParticipants_Users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssignmentParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsAnonymous = table.Column<bool>(type: "bit", nullable: false),
                    AllowMultipleResponses = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxResponses = table.Column<int>(type: "int", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_Users_CreatedByUserId",
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    CanViewSummary = table.Column<bool>(type: "bit", nullable: false),
                    CanViewRawResponses = table.Column<bool>(type: "bit", nullable: false),
                    CanViewParticipantIdentities = table.Column<bool>(type: "bit", nullable: false),
                    CanExportData = table.Column<bool>(type: "bit", nullable: false),
                    ViewingConfiguration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrantedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    GrantedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                name: "Questionnaires",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CurrentVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questionnaires", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questionnaires_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    SchemaJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VersionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SchemaHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Responses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnswersJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CompletionTimeMinutes = table.Column<int>(type: "int", nullable: true),
                    SubmissionIPAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnswersHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    IsFinalResponse = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Responses_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Responses_QuestionnaireVersions_QuestionnaireVersionId",
                        column: x => x.QuestionnaireVersionId,
                        principalTable: "QuestionnaireVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Responses_Users_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "IX_AssignmentParticipants_AddedByUserId",
                table: "AssignmentParticipants",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentParticipants_AssignmentId_UserId",
                table: "AssignmentParticipants",
                columns: new[] { "AssignmentId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentParticipants_HasResponded",
                table: "AssignmentParticipants",
                column: "HasResponded");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentParticipants_IsRequired",
                table: "AssignmentParticipants",
                column: "IsRequired");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentParticipants_UserId",
                table: "AssignmentParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CreatedByUserId",
                table: "Assignments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_QuestionnaireId",
                table: "Assignments",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_QuestionnaireVersionId",
                table: "Assignments",
                column: "QuestionnaireVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_Status",
                table: "Assignments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_Title",
                table: "Assignments",
                column: "Title");

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
                name: "IX_Questionnaires_CreatedByUserId",
                table: "Questionnaires",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_CurrentVersionId",
                table: "Questionnaires",
                column: "CurrentVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_Title",
                table: "Questionnaires",
                column: "Title");

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

            migrationBuilder.CreateIndex(
                name: "IX_Responses_AssignmentId",
                table: "Responses",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_AssignmentId_ParticipantId_SubmittedAt",
                table: "Responses",
                columns: new[] { "AssignmentId", "ParticipantId", "SubmittedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Responses_IsCompleted",
                table: "Responses",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_ParticipantId",
                table: "Responses",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_QuestionnaireVersionId",
                table: "Responses",
                column: "QuestionnaireVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_SubmittedAt",
                table: "Responses",
                column: "SubmittedAt");

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

            migrationBuilder.CreateIndex(
                name: "IX_Users_ActiveDirectoryGuid",
                table: "Users",
                column: "ActiveDirectoryGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentParticipants_Assignments_AssignmentId",
                table: "AssignmentParticipants",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_QuestionnaireVersions_QuestionnaireVersionId",
                table: "Assignments",
                column: "QuestionnaireVersionId",
                principalTable: "QuestionnaireVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Questionnaires_QuestionnaireId",
                table: "Assignments",
                column: "QuestionnaireId",
                principalTable: "Questionnaires",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questionnaires_QuestionnaireVersions_CurrentVersionId",
                table: "Questionnaires",
                column: "CurrentVersionId",
                principalTable: "QuestionnaireVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaires_Users_CreatedByUserId",
                table: "Questionnaires");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireVersions_Users_CreatedByUserId",
                table: "QuestionnaireVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaires_QuestionnaireVersions_CurrentVersionId",
                table: "Questionnaires");

            migrationBuilder.DropTable(
                name: "AssignmentParticipants");

            migrationBuilder.DropTable(
                name: "AssignmentViewers");

            migrationBuilder.DropTable(
                name: "Responses");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "QuestionnaireVersions");

            migrationBuilder.DropTable(
                name: "Questionnaires");
        }
    }
}
