using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeededDataWithBCryptHashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"),
                column: "PasswordHash",
                value: "$2a$11$R0p4oAB7hWPLdmU8pLRd1edAwWmoaki6roCXXPpzLNKgEwyvN2Wh.");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("f9e8d7c6-b5a4-3f2e-1d0c-9b8a7f6e5d4c"),
                column: "PasswordHash",
                value: "$2a$11$FuOGiOoclJgRJUBYRjAUhex3Cg8/woVKfwsQKRQR2fAaybH2ryV4W");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"),
                column: "PasswordHash",
                value: "008c70392e3abfbd0fa47bbc2ed96aa99bd49e159727fcba0f2e6abeb3a9d601");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("f9e8d7c6-b5a4-3f2e-1d0c-9b8a7f6e5d4c"),
                column: "PasswordHash",
                value: "5d1a707c451ebf55a9e2539978c6d91dd0c06f4d3020fc2a6be3e461a17f2081");
        }
    }
}
