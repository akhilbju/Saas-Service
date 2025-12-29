using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saas_Auth_Service.Migrations
{
    /// <inheritdoc />
    public partial class projectchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "Projects",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "Projects");
        }
    }
}
