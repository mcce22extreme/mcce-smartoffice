using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcce.SmartOffice.Bookings.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sobo");

            migrationBuilder.CreateTable(
                name: "Bookings",
                schema: "sobo",
                columns: table => new
                {
                    BookingNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activated = table.Column<bool>(type: "bit", nullable: false),
                    InvitationSent = table.Column<bool>(type: "bit", nullable: false),
                    WorkspaceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Creator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Modifier = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingNumber);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingNumber",
                schema: "sobo",
                table: "Bookings",
                column: "BookingNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings",
                schema: "sobo");
        }
    }
}
