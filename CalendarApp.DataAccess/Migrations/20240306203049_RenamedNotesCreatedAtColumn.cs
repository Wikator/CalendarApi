using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendarApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RenamedNotesCreatedAtColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Notes",
                newName: "LastModified");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastModified",
                table: "Notes",
                newName: "CreatedAt");
        }
    }
}
