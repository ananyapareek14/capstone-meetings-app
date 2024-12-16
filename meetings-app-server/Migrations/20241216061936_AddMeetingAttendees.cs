using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace meetings_server.Migrations
{
    /// <inheritdoc />
    public partial class AddMeetingAttendees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a71a55d6-99d7-4123-b4e0-1218ecb90e3e", "a71a55d6-99d7-4123-b4e0-1218ecb90e3e", "Reader", "READER" },
                    { "c309fa92-2123-47be-b397-a1c77adb502c", "c309fa92-2123-47be-b397-a1c77adb502c", "Writer", "WRITER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a71a55d6-99d7-4123-b4e0-1218ecb90e3e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c309fa92-2123-47be-b397-a1c77adb502c");
        }
    }
}
