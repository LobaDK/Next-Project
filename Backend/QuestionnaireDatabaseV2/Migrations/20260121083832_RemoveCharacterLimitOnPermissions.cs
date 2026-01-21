using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionnaireDatabaseV2.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCharacterLimitOnPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Permissions",
                table: "AssignmentParticipants",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Permissions",
                table: "AssignmentParticipants",
                type: "nvarchar(100)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
