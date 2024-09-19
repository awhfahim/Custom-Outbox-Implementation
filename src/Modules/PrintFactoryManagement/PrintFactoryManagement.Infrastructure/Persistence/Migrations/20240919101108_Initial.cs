using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrintFactoryManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "PFM");

            migrationBuilder.CreateTable(
                name: "InboxStates",
                schema: "PFM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MessageId = table.Column<string>(type: "NVARCHAR2(40)", maxLength: 40, nullable: false),
                    ProcessedOn = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "PFM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Payload = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    PayloadType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Status = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "PFM",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    UserName = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    ProfilePictureUri = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    DateOfBirth = table.Column<string>(type: "NVARCHAR2(10)", nullable: false),
                    Address = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "PFM",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                schema: "PFM",
                table: "Users",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                schema: "PFM",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InboxStates",
                schema: "PFM");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "PFM");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "PFM");
        }
    }
}
