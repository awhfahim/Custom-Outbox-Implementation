using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecurityManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ACM");

            migrationBuilder.CreateTable(
                name: "AuthorizablePermissionGroups",
                schema: "ACM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Label = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizablePermissionGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthorizableRoles",
                schema: "ACM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Label = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizableRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "ACM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    UserName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: false),
                    FullName = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    MaritalStatus = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Gender = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    ProfilePictureUri = table.Column<string>(type: "NCLOB", maxLength: 2048, nullable: true),
                    DateOfBirth = table.Column<string>(type: "NVARCHAR2(10)", nullable: false),
                    Address = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    IsArchived = table.Column<bool>(type: "NUMBER(1)", nullable: false, defaultValue: false),
                    ArchivedAtUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthorizablePermissions",
                schema: "ACM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    GroupId = table.Column<long>(type: "NUMBER(19)", nullable: true),
                    Label = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizablePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "Fk_AuthPermissions_Category",
                        column: x => x.GroupId,
                        principalSchema: "ACM",
                        principalTable: "AuthorizablePermissionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "ACM",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    AuthorizableRoleId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.AuthorizableRoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_AuthorizableRoles_AuthorizableRoleId",
                        column: x => x.AuthorizableRoleId,
                        principalSchema: "ACM",
                        principalTable: "AuthorizableRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "ACM",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                schema: "ACM",
                columns: table => new
                {
                    AuthorizableRoleId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    AuthorizablePermissionId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.AuthorizableRoleId, x.AuthorizablePermissionId });
                    table.ForeignKey(
                        name: "Fk_RolePermissions_AuthPermission",
                        column: x => x.AuthorizablePermissionId,
                        principalSchema: "ACM",
                        principalTable: "AuthorizablePermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Fk_RolePermissions_AuthRole",
                        column: x => x.AuthorizableRoleId,
                        principalSchema: "ACM",
                        principalTable: "AuthorizableRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizablePermissionGroups_Label",
                schema: "ACM",
                table: "AuthorizablePermissionGroups",
                column: "Label",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizablePermissions_GroupId",
                schema: "ACM",
                table: "AuthorizablePermissions",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizablePermissions_Label",
                schema: "ACM",
                table: "AuthorizablePermissions",
                column: "Label",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizableRoles_Label",
                schema: "ACM",
                table: "AuthorizableRoles",
                column: "Label",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_AuthorizablePermissionId",
                schema: "ACM",
                table: "RolePermissions",
                column: "AuthorizablePermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_AuthorizableRoleId",
                schema: "ACM",
                table: "UserRoles",
                column: "AuthorizableRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "ACM",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                schema: "ACM",
                table: "Users",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                schema: "ACM",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissions",
                schema: "ACM");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "ACM");

            migrationBuilder.DropTable(
                name: "AuthorizablePermissions",
                schema: "ACM");

            migrationBuilder.DropTable(
                name: "AuthorizableRoles",
                schema: "ACM");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "ACM");

            migrationBuilder.DropTable(
                name: "AuthorizablePermissionGroups",
                schema: "ACM");
        }
    }
}
