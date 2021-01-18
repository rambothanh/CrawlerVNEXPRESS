using Microsoft.EntityFrameworkCore.Migrations;

namespace CrawlerVNEXPRESS.Migrations.SqliteMigrations
{
    public partial class lan8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Captain",
                table: "ImageLinks",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Captain",
                table: "ImageLinks");
        }
    }
}
