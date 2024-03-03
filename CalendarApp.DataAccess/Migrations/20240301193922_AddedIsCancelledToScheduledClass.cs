using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendarApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsCancelledToScheduledClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "ScheduledClasses",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "ScheduledClasses");
        }
    }
}
