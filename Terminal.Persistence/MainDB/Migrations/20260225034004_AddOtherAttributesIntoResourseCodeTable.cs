using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Terminal.Persistence.MainDB.Migrations
{
    /// <inheritdoc />
    public partial class AddOtherAttributesIntoResourseCodeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Density",
                table: "resource_code",
                type: "NUMERIC(20,4)",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "IsShow",
                table: "resource_code",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<decimal>(
                name: "Temperature",
                table: "resource_code",
                type: "NUMERIC(20,4)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Density",
                table: "resource_code");

            migrationBuilder.DropColumn(
                name: "IsShow",
                table: "resource_code");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "resource_code");
        }
    }
}
