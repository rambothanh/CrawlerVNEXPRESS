using Microsoft.EntityFrameworkCore.Migrations;

namespace CrawlerVNEXPRESS.Migrations.SqliteMigrations
{
    public partial class Lan4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Newss_Categories_CategoryRefId",
                table: "Newss");

            migrationBuilder.DropIndex(
                name: "IX_Newss_CategoryRefId",
                table: "Newss");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Category");

            migrationBuilder.AddUniqueConstraint(
                name: "AlternateKey_Category",
                table: "Newss",
                column: "CategoryRefId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

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

            migrationBuilder.DropUniqueConstraint(
                name: "AlternateKey_Category",
                table: "Newss");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Categories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Newss_CategoryRefId",
                table: "Newss",
                column: "CategoryRefId");

            migrationBuilder.AddForeignKey(
                name: "FK_Newss_Categories_CategoryRefId",
                table: "Newss",
                column: "CategoryRefId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
