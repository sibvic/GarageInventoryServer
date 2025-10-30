using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarageInventoryServer.Migrations
{
    /// <inheritdoc />
    public partial class Projectlastindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastIndex",
                table: "Projects",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastIndex",
                table: "Projects");
        }
    }
}
