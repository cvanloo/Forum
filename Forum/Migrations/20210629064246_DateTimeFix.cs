using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum.Migrations
{
    public partial class DateTimeFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Joined",
                table: "UserForums",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 29, 8, 42, 45, 662, DateTimeKind.Local).AddTicks(2104),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 29, 8, 41, 31, 853, DateTimeKind.Local).AddTicks(7925));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Joined",
                table: "UserForums",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 29, 8, 41, 31, 853, DateTimeKind.Local).AddTicks(7925),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 29, 8, 42, 45, 662, DateTimeKind.Local).AddTicks(2104));
        }
    }
}
