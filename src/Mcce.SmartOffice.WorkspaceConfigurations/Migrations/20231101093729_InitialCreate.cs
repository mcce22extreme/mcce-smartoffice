using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcce.SmartOffice.WorkspaceConfigurations.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sowsc");

            migrationBuilder.CreateTable(
                name: "WorkspaceConfigurations",
                schema: "sowsc",
                columns: table => new
                {
                    ConfigurationNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DeskHeight = table.Column<long>(type: "bigint", nullable: false),
                    WorkspaceNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Creator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Modifier = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceConfigurations", x => x.ConfigurationNumber);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceConfigurations_WorkspaceNumber_UserName",
                schema: "sowsc",
                table: "WorkspaceConfigurations",
                columns: new[] { "WorkspaceNumber", "UserName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkspaceConfigurations",
                schema: "sowsc");
        }
    }
}
