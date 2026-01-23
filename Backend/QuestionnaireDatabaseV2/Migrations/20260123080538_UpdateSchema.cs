using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionnaireDatabaseV2.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowAnonymizedManagerViewing",
                table: "Assignments");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Assignments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Assignments");

            migrationBuilder.AddColumn<bool>(
                name: "AllowAnonymizedManagerViewing",
                table: "Assignments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
