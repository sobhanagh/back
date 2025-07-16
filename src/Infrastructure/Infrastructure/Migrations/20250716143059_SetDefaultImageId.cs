using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamaEdtech.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetDefaultImageId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE Schools SET DefaultImageId=(SELECT TOP 1 i.Id FROM SchoolImages i WHERE i.SchoolId = Id ORDER BY i.IsDefault DESC)
");
            migrationBuilder.Sql("UPDATE SchoolImages SET IsDefault = 1 WHERE Id IN (SELECT DefaultImageId FROM Schools");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //comment
        }
    }
}
