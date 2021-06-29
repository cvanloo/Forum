using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum.Migrations
{
    public partial class PwUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Joined",
                table: "UserForums",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 29, 12, 42, 29, 651, DateTimeKind.Local).AddTicks(7107),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 29, 11, 32, 39, 805, DateTimeKind.Local).AddTicks(8214));

            migrationBuilder.AddColumn<bool>(
                name: "Used",
                table: "PwResets",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Used",
                table: "PwResets");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Joined",
                table: "UserForums",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 29, 11, 32, 39, 805, DateTimeKind.Local).AddTicks(8214),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 29, 12, 42, 29, 651, DateTimeKind.Local).AddTicks(7107));
        }
    }
}
