using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SortSchedule.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCreatePermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Action", "Name", "Resource" },
                values: new object[] { new Guid("d1c83b20-4e9f-46b2-a03d-7e4f5a6b2d1c"), 1, "Create User", "User" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { new Guid("d1c83b20-4e9f-46b2-a03d-7e4f5a6b2d1c"), new Guid("6a8ebea7-0d17-4178-ad20-9ea0f0c2f771") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("d1c83b20-4e9f-46b2-a03d-7e4f5a6b2d1c"), new Guid("6a8ebea7-0d17-4178-ad20-9ea0f0c2f771") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("d1c83b20-4e9f-46b2-a03d-7e4f5a6b2d1c"));
        }
    }
}
