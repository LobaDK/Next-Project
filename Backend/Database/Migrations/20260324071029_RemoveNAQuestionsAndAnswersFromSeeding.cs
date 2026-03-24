using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNAQuestionsAndAnswersFromSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 7);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "QuestionnaireTemplateQuestion",
                columns: new[] { "Id", "AllowCustom", "Prompt", "QuestionnaireTemplateFK", "SortOrder" },
                values: new object[,]
                {
                    { 5, false, "N/A", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 4 },
                    { 6, false, "N/A", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 5 },
                    { 7, false, "N/A", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 6 }
                });

            migrationBuilder.InsertData(
                table: "QuestionnaireTemplateOption",
                columns: new[] { "Id", "DisplayText", "OptionValue", "QuestionFK", "SortOrder" },
                values: new object[,]
                {
                    { 21, "N/A", 1, 5, 0 },
                    { 22, "N/A", 1, 6, 0 },
                    { 23, "N/A", 1, 7, 0 }
                });
        }
    }
}
