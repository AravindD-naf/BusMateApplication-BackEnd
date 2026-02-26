using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusTicketingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AdminPasswordHashUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$12$q3IlCTLc4..NdjaxvVHL.OVvMulc/lMJ306tHOJ0gXufE3GGdho76");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$FjK8XhGd9jF2dLk3nA5b6O2bJx9x...");
        }
    }
}
