using Microsoft.EntityFrameworkCore.Migrations;

namespace CrawlerVNEXPRESS.Migrations.SqliteMigrations
{
    public partial class Lan6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Location",
                table: "ImageLink",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Location",
                table: "Content",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "ImageLink");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Content");
        }
    }
}
