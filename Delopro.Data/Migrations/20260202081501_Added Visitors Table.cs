using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Delopro.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedVisitorsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Users_UserId",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "Visits");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Visits",
                newName: "VisitorId");

            migrationBuilder.RenameIndex(
                name: "IX_Visits_UserId",
                table: "Visits",
                newName: "IX_Visits_VisitorId");

            migrationBuilder.CreateTable(
                name: "Visitors",
                columns: table => new
                {
                    VisitorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitors", x => x.VisitorId);
                    table.ForeignKey(
                        name: "FK_Visitors_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.InsertData(
                table: "Visitors",
                columns: new[] { "VisitorId", "City", "Country", "IpAddress", "UserId" },
                values: new object[,]
                {
                    { 1, "Minsk", "Belarus", "37.214.25.23", 2 },
                    { 2, "Minsk", "Belarus", "46.216.112.76", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_UserId",
                table: "Visitors",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Visitors_VisitorId",
                table: "Visits",
                column: "VisitorId",
                principalTable: "Visitors",
                principalColumn: "VisitorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Visitors_VisitorId",
                table: "Visits");

            migrationBuilder.DropTable(
                name: "Visitors");

            migrationBuilder.RenameColumn(
                name: "VisitorId",
                table: "Visits",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Visits_VisitorId",
                table: "Visits",
                newName: "IX_Visits_UserId");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Visits",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Visits",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "Visits",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Users_UserId",
                table: "Visits",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
