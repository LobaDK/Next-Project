using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionnaireDatabaseV2.Migrations
{
    /// <inheritdoc />
    public partial class AddSupportForGranularPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Permissions",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AuxiliaryRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AddedPermissions = table.Column<int>(type: "int", nullable: false),
                    RemovedPermissions = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuxiliaryRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuxiliaryRoleUser",
                columns: table => new
                {
                    AuxiliaryRolesId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuxiliaryRoleUser", x => new { x.AuxiliaryRolesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_AuxiliaryRoleUser_AuxiliaryRole_AuxiliaryRolesId",
                        column: x => x.AuxiliaryRolesId,
                        principalTable: "AuxiliaryRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuxiliaryRoleUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuxiliaryRole_Name",
                table: "AuxiliaryRole",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuxiliaryRoleUser_UsersId",
                table: "AuxiliaryRoleUser",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuxiliaryRoleUser");

            migrationBuilder.DropTable(
                name: "AuxiliaryRole");

            migrationBuilder.DropColumn(
                name: "Permissions",
                table: "Users");
        }
    }
}
