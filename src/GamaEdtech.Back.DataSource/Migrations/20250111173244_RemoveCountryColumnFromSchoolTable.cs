using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamaEdtech.Back.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCountryColumnFromSchoolTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressCountry",
                table: "School");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressCountry",
                table: "School",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
