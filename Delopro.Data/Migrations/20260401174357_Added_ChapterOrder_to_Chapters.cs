using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delopro.Data.Migrations
{
    /// <inheritdoc />
    public partial class Added_ChapterOrder_to_Chapters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChapterOrder",
                table: "Chapters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chapter_Order",
                table: "Chapters",
                column: "ChapterOrder",
                unique: true,
                filter: "[ChapterOrder] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Chapter_Order",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "ChapterOrder",
                table: "Chapters");
        }
    }
}
