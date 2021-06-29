using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum.Migrations
{
    public partial class PwResetUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PwReset_Users_UserId",
                table: "PwReset");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PwReset",
                table: "PwReset");

            migrationBuilder.RenameTable(
                name: "PwReset",
                newName: "PwResets");

            migrationBuilder.RenameIndex(
                name: "IX_PwReset_UserId",
                table: "PwResets",
                newName: "IX_PwResets_UserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Joined",
                table: "UserForums",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 29, 11, 32, 39, 805, DateTimeKind.Local).AddTicks(8214),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 29, 11, 11, 56, 536, DateTimeKind.Local).AddTicks(9191));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "PwResets",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PwResets",
                table: "PwResets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PwResets_Users_UserId",
                table: "PwResets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PwResets_Users_UserId",
                table: "PwResets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PwResets",
                table: "PwResets");

            migrationBuilder.RenameTable(
                name: "PwResets",
                newName: "PwReset");

            migrationBuilder.RenameIndex(
                name: "IX_PwResets_UserId",
                table: "PwReset",
                newName: "IX_PwReset_UserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Joined",
                table: "UserForums",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 29, 11, 11, 56, 536, DateTimeKind.Local).AddTicks(9191),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 29, 11, 32, 39, 805, DateTimeKind.Local).AddTicks(8214));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "PwReset",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PwReset",
                table: "PwReset",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PwReset_Users_UserId",
                table: "PwReset",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
