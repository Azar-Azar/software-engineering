using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace software_engineering.Migrations
{
    /// <inheritdoc />
    public partial class AddPressureData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PressureData",
                columns: table => new
                {
                    DataID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RawData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PeakPressureIndex = table.Column<float>(type: "real", nullable: false),
                    ContactAreaPercentage = table.Column<float>(type: "real", nullable: false),
                    IsHighPressure = table.Column<bool>(type: "bit", nullable: false),
                    FlaggedForReview = table.Column<bool>(type: "bit", nullable: false),
                    ReviewNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PressureData", x => x.DataID);
                    table.ForeignKey(
                        name: "FK_PressureData_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PressureData_UserID",
                table: "PressureData",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PressureData");
        }
    }
}
