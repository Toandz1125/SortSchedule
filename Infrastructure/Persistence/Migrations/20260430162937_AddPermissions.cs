using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SortSchedule.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Resource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Effect = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_AppRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AppRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Action", "Name", "Resource" },
                values: new object[,]
                {
                    { new Guid("61a5c1f0-8041-456a-8e4e-3bc38a2b0e76"), 2, "Update Schedule", "Schedule" },
                    { new Guid("92a3a69a-40d9-4db8-8f2b-df9d23ed1d27"), 5, "Manage Schedule", "Schedule" },
                    { new Guid("a2e7f3d0-4b19-4a92-9c21-6a2b3c4d5e6f"), 1, "Create Feedback", "Feedback" },
                    { new Guid("c4b72a1f-3d8e-45a1-9f2c-6d3e4f5a1b2c"), 0, "Read Feedback", "Feedback" },
                    { new Guid("d1a562b1-4a59-42e1-9d67-33c87332c19e"), 0, "Read Schedule", "Schedule" },
                    { new Guid("e5c83b20-4e9f-46b2-a03d-7e4f5a6b2c3d"), 2, "Update Feedback", "Feedback" },
                    { new Guid("f8e1c2e6-b94f-4c82-936a-b6a78d54a43d"), 1, "Create Free Time", "FreeTime" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("d1a562b1-4a59-42e1-9d67-33c87332c19e"), new Guid("2f5b4f8e-7659-4955-bad6-b526639c3cdb") },
                    { new Guid("a2e7f3d0-4b19-4a92-9c21-6a2b3c4d5e6f"), new Guid("4f7d6a97-b228-4a62-a308-91530c223a84") },
                    { new Guid("d1a562b1-4a59-42e1-9d67-33c87332c19e"), new Guid("4f7d6a97-b228-4a62-a308-91530c223a84") },
                    { new Guid("f8e1c2e6-b94f-4c82-936a-b6a78d54a43d"), new Guid("4f7d6a97-b228-4a62-a308-91530c223a84") },
                    { new Guid("61a5c1f0-8041-456a-8e4e-3bc38a2b0e76"), new Guid("6a8ebea7-0d17-4178-ad20-9ea0f0c2f771") },
                    { new Guid("92a3a69a-40d9-4db8-8f2b-df9d23ed1d27"), new Guid("6a8ebea7-0d17-4178-ad20-9ea0f0c2f771") },
                    { new Guid("c4b72a1f-3d8e-45a1-9f2c-6d3e4f5a1b2c"), new Guid("6a8ebea7-0d17-4178-ad20-9ea0f0c2f771") },
                    { new Guid("d1a562b1-4a59-42e1-9d67-33c87332c19e"), new Guid("6a8ebea7-0d17-4178-ad20-9ea0f0c2f771") },
                    { new Guid("e5c83b20-4e9f-46b2-a03d-7e4f5a6b2c3d"), new Guid("6a8ebea7-0d17-4178-ad20-9ea0f0c2f771") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Resource_Action",
                table: "Permissions",
                columns: new[] { "Resource", "Action" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Permissions");
        }
    }
}
