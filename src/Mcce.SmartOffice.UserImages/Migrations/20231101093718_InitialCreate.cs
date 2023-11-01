using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcce.SmartOffice.UserImages.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sousi");

            migrationBuilder.CreateTable(
                name: "UserImages",
                schema: "sousi",
                columns: table => new
                {
                    ImageKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserImages", x => x.ImageKey);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserImages",
                schema: "sousi");
        }
    }
}
