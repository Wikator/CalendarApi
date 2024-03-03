using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendarApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MadeUserFacultiesNullabled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Subjects_Faculty1Id",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Subjects_Faculty2Id",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Subjects_Faculty3Id",
                table: "Users");

            migrationBuilder.AlterColumn<uint>(
                name: "Faculty3Id",
                table: "Users",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(uint),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<uint>(
                name: "Faculty2Id",
                table: "Users",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(uint),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<uint>(
                name: "Faculty1Id",
                table: "Users",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(uint),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Subjects_Faculty1Id",
                table: "Users",
                column: "Faculty1Id",
                principalTable: "Subjects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Subjects_Faculty2Id",
                table: "Users",
                column: "Faculty2Id",
                principalTable: "Subjects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Subjects_Faculty3Id",
                table: "Users",
                column: "Faculty3Id",
                principalTable: "Subjects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Subjects_Faculty1Id",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Subjects_Faculty2Id",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Subjects_Faculty3Id",
                table: "Users");

            migrationBuilder.AlterColumn<uint>(
                name: "Faculty3Id",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u,
                oldClrType: typeof(uint),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "Faculty2Id",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u,
                oldClrType: typeof(uint),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<uint>(
                name: "Faculty1Id",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u,
                oldClrType: typeof(uint),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Subjects_Faculty1Id",
                table: "Users",
                column: "Faculty1Id",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Subjects_Faculty2Id",
                table: "Users",
                column: "Faculty2Id",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Subjects_Faculty3Id",
                table: "Users",
                column: "Faculty3Id",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
