using Microsoft.EntityFrameworkCore.Migrations;

namespace CrawlerVNEXPRESS.Migrations.SqliteMigrations
{
    public partial class lan7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Content_Newss_NewsRefId",
                table: "Content");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageLink_Newss_NewsRefId",
                table: "ImageLink");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageLink",
                table: "ImageLink");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Content",
                table: "Content");

            migrationBuilder.RenameTable(
                name: "ImageLink",
                newName: "ImageLinks");

            migrationBuilder.RenameTable(
                name: "Content",
                newName: "Contents");

            migrationBuilder.RenameIndex(
                name: "IX_ImageLink_NewsRefId",
                table: "ImageLinks",
                newName: "IX_ImageLinks_NewsRefId");

            migrationBuilder.RenameIndex(
                name: "IX_Content_NewsRefId",
                table: "Contents",
                newName: "IX_Contents_NewsRefId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageLinks",
                table: "ImageLinks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contents",
                table: "Contents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_Newss_NewsRefId",
                table: "Contents",
                column: "NewsRefId",
                principalTable: "Newss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageLinks_Newss_NewsRefId",
                table: "ImageLinks",
                column: "NewsRefId",
                principalTable: "Newss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_Newss_NewsRefId",
                table: "Contents");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageLinks_Newss_NewsRefId",
                table: "ImageLinks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageLinks",
                table: "ImageLinks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contents",
                table: "Contents");

            migrationBuilder.RenameTable(
                name: "ImageLinks",
                newName: "ImageLink");

            migrationBuilder.RenameTable(
                name: "Contents",
                newName: "Content");

            migrationBuilder.RenameIndex(
                name: "IX_ImageLinks_NewsRefId",
                table: "ImageLink",
                newName: "IX_ImageLink_NewsRefId");

            migrationBuilder.RenameIndex(
                name: "IX_Contents_NewsRefId",
                table: "Content",
                newName: "IX_Content_NewsRefId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageLink",
                table: "ImageLink",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Content",
                table: "Content",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Content_Newss_NewsRefId",
                table: "Content",
                column: "NewsRefId",
                principalTable: "Newss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageLink_Newss_NewsRefId",
                table: "ImageLink",
                column: "NewsRefId",
                principalTable: "Newss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
