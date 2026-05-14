using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Terminal.Data.MainDB.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceCodeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "resource_code",
                columns: table => new
                {
                    FuelCodeKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CollectionKey = table.Column<int>(type: "INTEGER", nullable: false),
                    ResourceKey = table.Column<int>(type: "INTEGER", nullable: false),
                    ResourceName = table.Column<string>(type: "TEXT", nullable: true),
                    ResourcePrice = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resource_code", x => x.FuelCodeKey);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "resource_code");
        }
    }
}
