using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecurityManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "ACM",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "ACM");
        }
    }
}
