using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace GamaEdtech.Back.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "School",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameInEnglish = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameInLocalLanguage = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    AddressDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AddressGeography = table.Column<Point>(type: "GEOGRAPHY", nullable: false),
                    AddressCountry = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AddressState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AddressCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AddressZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_School", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "School");
        }
    }
}
