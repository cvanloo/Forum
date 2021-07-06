using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum.Migrations
{
    public partial class AddedFKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Joined",
                table: "UserForums",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 29, 12, 42, 29, 651, DateTimeKind.Local).AddTicks(7107));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Joined",
                table: "UserForums",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 29, 12, 42, 29, 651, DateTimeKind.Local).AddTicks(7107),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValueSql: "NOW()");
        }
    }
}
