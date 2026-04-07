using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogLevel = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    EventDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    LastUpated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    TemplateStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PrimaryRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Permissions = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireGroups",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireGroups", x => x.GroupId);
                    table.ForeignKey(
                        name: "FK_QuestionnaireGroups_QuestionnaireTemplate_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "QuestionnaireTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireTemplateQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prompt = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AllowCustom = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    QuestionnaireTemplateFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireTemplateQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireTemplateQuestion_QuestionnaireTemplate_QuestionnaireTemplateFK",
                        column: x => x.QuestionnaireTemplateFK,
                        principalTable: "QuestionnaireTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackedRefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<byte[]>(type: "varbinary(900)", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    UserBaseModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedRefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackedRefreshToken_User_UserBaseModelId",
                        column: x => x.UserBaseModelId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ActiveQuestionnaire",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudentFK = table.Column<int>(type: "int", nullable: false),
                    TeacherFK = table.Column<int>(type: "int", nullable: false),
                    QuestionnaireTemplateFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    StudentCompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TeacherCompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveQuestionnaire", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaire_QuestionnaireGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "QuestionnaireGroups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaire_QuestionnaireTemplate_QuestionnaireTemplateFK",
                        column: x => x.QuestionnaireTemplateFK,
                        principalTable: "QuestionnaireTemplate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaire_User_StudentFK",
                        column: x => x.StudentFK,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaire_User_TeacherFK",
                        column: x => x.TeacherFK,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireTemplateOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OptionValue = table.Column<int>(type: "int", nullable: false),
                    DisplayText = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    QuestionFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireTemplateOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireTemplateOption_QuestionnaireTemplateQuestion_QuestionFK",
                        column: x => x.QuestionFK,
                        principalTable: "QuestionnaireTemplateQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActiveQuestionnaireResponse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionFK = table.Column<int>(type: "int", nullable: false),
                    OptionFK = table.Column<int>(type: "int", nullable: true),
                    CustomResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActiveQuestionnaireFK = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(55)", maxLength: 55, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveQuestionnaireResponse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaireResponse_ActiveQuestionnaire_ActiveQuestionnaireFK",
                        column: x => x.ActiveQuestionnaireFK,
                        principalTable: "ActiveQuestionnaire",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaireResponse_QuestionnaireTemplateOption_OptionFK",
                        column: x => x.OptionFK,
                        principalTable: "QuestionnaireTemplateOption",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActiveQuestionnaireResponse_QuestionnaireTemplateQuestion_QuestionFK",
                        column: x => x.QuestionFK,
                        principalTable: "QuestionnaireTemplateQuestion",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "QuestionnaireTemplate",
                columns: new[] { "Id", "Description", "TemplateStatus", "Title" },
                values: new object[] { new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), "Gennemførelsesprocedure for SKP-elever ved PRAKTIK NORD", 0, "Evaluering af SKP-elever" });

            migrationBuilder.InsertData(
                table: "QuestionnaireTemplateQuestion",
                columns: new[] { "Id", "AllowCustom", "Prompt", "QuestionnaireTemplateFK", "SortOrder" },
                values: new object[,]
                {
                    { 1, false, "Indlæringsevne", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 0 },
                    { 2, false, "Kreativitet og selvstændighed", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 1 },
                    { 3, false, "Arbejdsindsats", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 2 },
                    { 4, false, "Orden og omhyggelighed", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 3 },
                    { 8, false, "Mødestabilitet", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 7 },
                    { 9, false, "Sygdom", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 8 },
                    { 10, false, "Fravær", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 9 },
                    { 11, false, "Praktikpladssøgning", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 10 },
                    { 12, false, "Synlighed", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 11 }
                });

            migrationBuilder.InsertData(
                table: "QuestionnaireTemplateOption",
                columns: new[] { "Id", "DisplayText", "OptionValue", "QuestionFK", "SortOrder" },
                values: new object[,]
                {
                    { 1, "Viser lidt eller ingen forståelse for arbejdsopgaverne.", 1, 1, 0 },
                    { 2, "Forstår arbejdsopgaverne, men kan ikke anvende den i praksis. Har svært ved at tilegne sig ny viden.", 2, 1, 1 },
                    { 3, "Let ved at forstå arbejdsopgaverne og anvende den i praksis. Har let ved at tilegne sig ny viden.", 3, 1, 2 },
                    { 4, "Mindre behov for oplæring end normalt. Kan selv finde/tilegne sig ny viden.", 4, 1, 3 },
                    { 5, "Behøver næsten ingen oplæring. Kan ved selvstudium, endog ved svært tilgængeligt materiale, tilegne sig ny viden.", 5, 1, 4 },
                    { 6, "Viser intet initiativ. Er passiv, uinteresseret og uselvstændig.", 1, 2, 0 },
                    { 7, "Viser ringe initiativ. Kommer ikke selv med løsningsforslag. Viser ingen interesse i at tilrettelægge eget arbejde.", 2, 2, 1 },
                    { 8, "Viser normalt initiativ. Kommer selv med løsningsforslag. Tilrettelægger eget arbejde.", 3, 2, 2 },
                    { 9, "Meget initiativrig. Kommer selv med løsningsforslag. Gode evner for at tilrettelægge eget og andres arbejde.", 4, 2, 3 },
                    { 10, "Overordentlig initiativrig. Løser selv problemerne. Tilrettelægger selvstændigt arbejdet for mig selv og andre.", 5, 2, 4 },
                    { 11, "Uacceptabel", 1, 3, 0 },
                    { 12, "Under middel", 2, 3, 1 },
                    { 13, "Middel", 3, 3, 2 },
                    { 14, "Over middel", 4, 3, 3 },
                    { 15, "Særdeles god", 5, 3, 4 },
                    { 16, "Omgås materialer, maskiner og værktøj på en sløset og ligegyldig måde. Holder ikke sin arbejdsplads ordentlig.", 1, 4, 0 },
                    { 17, "Bruger maskiner og værktøj uden megen omtanke. Mindre god orden og omhyggelighed.", 2, 4, 1 },
                    { 18, "Påpasselighed og omhyggelighed middel. Rimelig god orden.", 3, 4, 2 },
                    { 19, "Meget påpasselig både i praktik og teori. God orden.", 4, 4, 3 },
                    { 20, "I høj grad påpasselig. God forståelse for materialevalg. Særdeles god orden.", 5, 4, 4 },
                    { 24, "Du møder ikke hver dag til tiden.", 1, 8, 0 },
                    { 25, "Du møder næsten hver dag til tiden.", 2, 8, 1 },
                    { 26, "Du møder hver dag til tiden.", 3, 8, 2 },
                    { 27, "Du melder ikke afbud ved sygdom.", 1, 9, 0 },
                    { 28, "Du melder, for det meste afbud, når du er syg.", 2, 9, 1 },
                    { 29, "Du melder afbud, når du er syg.", 3, 9, 2 },
                    { 30, "Du har et stort fravær.", 1, 10, 0 },
                    { 31, "Du har noget fravær.", 2, 10, 1 },
                    { 32, "Du har stort set ingen fravær.", 3, 10, 2 },
                    { 33, "Du har ingen fravær.", 4, 10, 3 },
                    { 34, "Du søger ingen praktikpladser.", 1, 11, 0 },
                    { 35, "Du ved, at du skal søge alle relevante praktikpladser, men det kniber med handlingen.", 2, 11, 1 },
                    { 36, "Du søger alle relevante praktikpladser, men skal have hjælp til at søge praktikpladser, der ligger længere væk end i din bopælskommune.", 3, 11, 2 },
                    { 37, "Du søger alle relevante praktikpladser også dem der ligger uden for din bopælskommune.", 4, 11, 3 },
                    { 38, "Du søger alle relevante praktikpladser også dem der ligger uden for din bopælskommune. Du søger også praktikplads inden for en anden uddannelse, som dit GF giver adgang til.", 5, 11, 4 },
                    { 39, "Du har ikke en synlig profil på praktikpladsen.dk.", 1, 12, 0 },
                    { 40, "Du skal ofte påmindes om at synliggøre din profil på praktikpladsen.dk.", 2, 12, 1 },
                    { 41, "Du har altid en synlig, men ikke opdateret profil på praktikpladsen.dk.", 3, 12, 2 },
                    { 42, "Du har altid en opdateret og synlig profil på praktikpladsen.dk.", 4, 12, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaire_GroupId",
                table: "ActiveQuestionnaire",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaire_QuestionnaireTemplateFK",
                table: "ActiveQuestionnaire",
                column: "QuestionnaireTemplateFK");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaire_StudentFK",
                table: "ActiveQuestionnaire",
                column: "StudentFK");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaire_TeacherFK",
                table: "ActiveQuestionnaire",
                column: "TeacherFK");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaire_Title",
                table: "ActiveQuestionnaire",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireResponse_ActiveQuestionnaireFK",
                table: "ActiveQuestionnaireResponse",
                column: "ActiveQuestionnaireFK");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireResponse_OptionFK",
                table: "ActiveQuestionnaireResponse",
                column: "OptionFK");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveQuestionnaireResponse_QuestionFK",
                table: "ActiveQuestionnaireResponse",
                column: "QuestionFK");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireGroups_TemplateId",
                table: "QuestionnaireGroups",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireTemplate_CreatedAt_Id",
                table: "QuestionnaireTemplate",
                columns: new[] { "CreatedAt", "Id" },
                descending: new[] { true, false });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireTemplate_Title",
                table: "QuestionnaireTemplate",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireTemplate_Title_Id",
                table: "QuestionnaireTemplate",
                columns: new[] { "Title", "Id" },
                descending: new[] { true, false });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireTemplateOption_QuestionFK",
                table: "QuestionnaireTemplateOption",
                column: "QuestionFK");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireTemplateQuestion_QuestionnaireTemplateFK",
                table: "QuestionnaireTemplateQuestion",
                column: "QuestionnaireTemplateFK");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedRefreshToken_Token",
                table: "TrackedRefreshToken",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedRefreshToken_UserBaseModelId",
                table: "TrackedRefreshToken",
                column: "UserBaseModelId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Guid",
                table: "User",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_UserName",
                table: "User",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveQuestionnaireResponse");

            migrationBuilder.DropTable(
                name: "ApplicationLogs");

            migrationBuilder.DropTable(
                name: "TrackedRefreshToken");

            migrationBuilder.DropTable(
                name: "ActiveQuestionnaire");

            migrationBuilder.DropTable(
                name: "QuestionnaireTemplateOption");

            migrationBuilder.DropTable(
                name: "QuestionnaireGroups");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "QuestionnaireTemplateQuestion");

            migrationBuilder.DropTable(
                name: "QuestionnaireTemplate");
        }
    }
}
