using Microsoft.EntityFrameworkCore.Migrations;

namespace CrawlerVNEXPRESS.Migrations.SqliteMigrations
{
    public partial class Lan2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Newss");

            migrationBuilder.AddColumn<long>(
                name: "CategoryRefId",
                table: "Newss",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Newss_CategoryRefId",
                table: "Newss",
                column: "CategoryRefId");

            migrationBuilder.AddForeignKey(
                name: "FK_Newss_Category_CategoryRefId",
                table: "Newss",
                column: "CategoryRefId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Newss_Category_CategoryRefId",
                table: "Newss");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Newss_CategoryRefId",
                table: "Newss");

            migrationBuilder.DropColumn(
                name: "CategoryRefId",
                table: "Newss");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Newss",
                type: "TEXT",
                nullable: true);
        }
    }
}
