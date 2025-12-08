using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace software_engineering.Migrations
{
    /// <inheritdoc />
    public partial class Acknowledge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Acknowledge",
                table: "Alerts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Acknowledge",
                table: "Alerts");
        }
    }
}
