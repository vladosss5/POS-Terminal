using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Terminal.Data.EventDB.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "incass",
                columns: table => new
                {
                    IncassKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LastDatetimeStart = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    LastDatetimeEnd = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    Flags = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_incass", x => x.IncassKey);
                });

            migrationBuilder.CreateTable(
                name: "ProtocolFilingForm",
                columns: table => new
                {
                    ProtokolFillingFormKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    sncProjectKey = table.Column<int>(type: "INTEGER", nullable: true),
                    LogCode = table.Column<int>(type: "INTEGER", nullable: true),
                    PlaceID = table.Column<long>(type: "NUMERIC(20)", nullable: true),
                    SubjectType = table.Column<int>(type: "NUMERIC(20)", nullable: true),
                    SubjectID = table.Column<int>(type: "NUMERIC(20)", nullable: true),
                    ObjectType = table.Column<int>(type: "NUMERIC(20)", nullable: true),
                    ObjectID = table.Column<long>(type: "NUMERIC(20)", nullable: true),
                    EventCode = table.Column<int>(type: "INTEGER", nullable: true),
                    EventKey = table.Column<long>(type: "NUMERIC(20)", nullable: true),
                    EventValue = table.Column<double>(type: "DOUBLE", nullable: true),
                    EventInfo = table.Column<string>(type: "TEXT", nullable: true),
                    EventDatetime = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    LatestObjectParameterValue = table.Column<double>(type: "DOUBLE", nullable: true),
                    CurrentObjectParameterValue = table.Column<double>(type: "DOUBLE", nullable: true),
                    Hash = table.Column<string>(type: "varchar(50)", nullable: true),
                    ErrorCode = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtocolFilingForm", x => x.ProtokolFillingFormKey);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "incass");

            migrationBuilder.DropTable(
                name: "ProtocolFilingForm");
        }
    }
}
