using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcce.SmartOffice.WorkspaceDataEntries.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sowsd");

            migrationBuilder.CreateTable(
                name: "WorkspaceDataEntries",
                schema: "sowsd",
                columns: table => new
                {
                    EntryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WorkspaceNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Wei = table.Column<int>(type: "int", nullable: false),
                    Temperature = table.Column<float>(type: "real", nullable: false),
                    Co2Level = table.Column<float>(type: "real", nullable: false),
                    Humidity = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceDataEntries", x => x.EntryId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceDataEntries_WorkspaceNumber",
                schema: "sowsd",
                table: "WorkspaceDataEntries",
                column: "WorkspaceNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkspaceDataEntries",
                schema: "sowsd");
        }
    }
}
