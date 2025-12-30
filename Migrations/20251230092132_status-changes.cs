using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saas_Auth_Service.Migrations
{
    /// <inheritdoc />
    public partial class statuschanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Statuses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Statuses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Statuses");
        }
    }
}
