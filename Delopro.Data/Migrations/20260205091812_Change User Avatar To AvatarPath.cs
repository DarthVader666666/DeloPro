using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Delopro.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserAvatarToAvatarPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "AvatarPath",
                table: "Users",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "AvatarPath",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "AvatarPath",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarPath",
                table: "Users");

            migrationBuilder.AddColumn<byte[]>(
                name: "Avatar",
                table: "Users",
                type: "varbinary(8000)",
                maxLength: 8000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Avatar",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Avatar",
                value: null);
        }
    }
}
