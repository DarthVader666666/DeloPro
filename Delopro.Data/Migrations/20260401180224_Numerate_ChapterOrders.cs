using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delopro.Data.Migrations
{
    /// <inheritdoc />
    public partial class Numerate_ChapterOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Chapters SET ChapterOrder = x.NewOrder FROM " +
                "(SELECT ChapterId, ROW_NUMBER() OVER (ORDER BY ChapterId) AS NewOrder FROM Chapters " +
                "WHERE ChapterOrder IS NULL) AS x WHERE Chapters.ChapterId = x.ChapterId;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Chapters SET ChapterOrder = NULL");
        }
    }
}
