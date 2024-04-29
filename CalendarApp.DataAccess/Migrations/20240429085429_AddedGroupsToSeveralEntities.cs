using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendarApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedGroupsToSeveralEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Group",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Group",
                table: "Tests",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Group",
                table: "ScheduledClasses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Group",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "ScheduledClasses");
        }
    }
}
