using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum.Migrations
{
    public partial class withuniqueagain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 28, 16, 2, 11, 681, DateTimeKind.Local).AddTicks(6044),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 28, 15, 58, 18, 551, DateTimeKind.Local).AddTicks(9919));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Joined",
                table: "UserForums",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 28, 16, 2, 11, 716, DateTimeKind.Local).AddTicks(8702),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 28, 15, 58, 18, 584, DateTimeKind.Local).AddTicks(8208));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Threads",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 28, 16, 2, 11, 716, DateTimeKind.Local).AddTicks(1945),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 28, 15, 58, 18, 584, DateTimeKind.Local).AddTicks(4506));

            migrationBuilder.AlterColumn<string>(
                name: "Identifier",
                table: "Sessions",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Forums",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 28, 16, 2, 11, 715, DateTimeKind.Local).AddTicks(9071),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 28, 15, 58, 18, 584, DateTimeKind.Local).AddTicks(2981));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Comments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 28, 16, 2, 11, 716, DateTimeKind.Local).AddTicks(5669),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 28, 15, 58, 18, 584, DateTimeKind.Local).AddTicks(6471));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Sent",
                table: "ChatMessage",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 28, 16, 2, 11, 717, DateTimeKind.Local).AddTicks(5913),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 28, 15, 58, 18, 585, DateTimeKind.Local).AddTicks(4675));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Chat",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 28, 16, 2, 11, 717, DateTimeKind.Local).AddTicks(4065),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 28, 15, 58, 18, 585, DateTimeKind.Local).AddTicks(2902));

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId_Identifier",
                table: "Sessions",
                columns: new[] { "UserId", "Identifier" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sessions_UserId_Identifier",
                table: "Sessions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 28, 15, 58, 18, 551, DateTimeKind.Local).AddTicks(9919),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 28, 16, 2, 11, 681, DateTimeKind.Local).AddTicks(6044));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Joined",
                table: "UserForums",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 28, 15, 58, 18, 584, DateTimeKind.Local).AddTicks(8208),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 28, 16, 2, 11, 716, DateTimeKind.Local).AddTicks(8702));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Threads",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 28, 15, 58, 18, 584, DateTimeKind.Local).AddTicks(4506),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 28, 16, 2, 11, 716, DateTimeKind.Local).AddTicks(1945));

            migrationBuilder.AlterColumn<string>(
                name: "Identifier",
                table: "Sessions",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Forums",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 28, 15, 58, 18, 584, DateTimeKind.Local).AddTicks(2981),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 28, 16, 2, 11, 715, DateTimeKind.Local).AddTicks(9071));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Comments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 28, 15, 58, 18, 584, DateTimeKind.Local).AddTicks(6471),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 28, 16, 2, 11, 716, DateTimeKind.Local).AddTicks(5669));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Sent",
                table: "ChatMessage",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 28, 15, 58, 18, 585, DateTimeKind.Local).AddTicks(4675),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 28, 16, 2, 11, 717, DateTimeKind.Local).AddTicks(5913));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Chat",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2021, 6, 28, 15, 58, 18, 585, DateTimeKind.Local).AddTicks(2902),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2021, 6, 28, 16, 2, 11, 717, DateTimeKind.Local).AddTicks(4065));
        }
    }
}
