using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"),
                columns: new[] { "AccountNumber", "Active" },
                values: new object[] { 100, true });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("f9e8d7c6-b5a4-3f2e-1d0c-9b8a7f6e5d4c"),
                columns: new[] { "AccountNumber", "Active" },
                values: new object[] { 150, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"),
                columns: new[] { "AccountNumber", "Active" },
                values: new object[] { 0, false });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("f9e8d7c6-b5a4-3f2e-1d0c-9b8a7f6e5d4c"),
                columns: new[] { "AccountNumber", "Active" },
                values: new object[] { 0, false });
        }
    }
}
