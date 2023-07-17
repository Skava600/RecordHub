using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecordHub.CatalogService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class recordstyleupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Languages_LanguageId",
                table: "Records");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "RecordStyles");

            migrationBuilder.RenameColumn(
                name: "LanguageId",
                table: "Records",
                newName: "CountryId");

            migrationBuilder.RenameIndex(
                name: "IX_Records_LanguageId",
                table: "Records",
                newName: "IX_Records_CountryId");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Styles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Records",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Labels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Artists",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecordStyle",
                columns: table => new
                {
                    RecordsId = table.Column<Guid>(type: "uuid", nullable: false),
                    StylesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordStyle", x => new { x.RecordsId, x.StylesId });
                    table.ForeignKey(
                        name: "FK_RecordStyle_Records_RecordsId",
                        column: x => x.RecordsId,
                        principalTable: "Records",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecordStyle_Styles_StylesId",
                        column: x => x.StylesId,
                        principalTable: "Styles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordStyle_StylesId",
                table: "RecordStyle",
                column: "StylesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Countries_CountryId",
                table: "Records",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Countries_CountryId",
                table: "Records");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "RecordStyle");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Styles");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Artists");

            migrationBuilder.RenameColumn(
                name: "CountryId",
                table: "Records",
                newName: "LanguageId");

            migrationBuilder.RenameIndex(
                name: "IX_Records_CountryId",
                table: "Records",
                newName: "IX_Records_LanguageId");

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecordStyles",
                columns: table => new
                {
                    StyleId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecordId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordStyles", x => new { x.StyleId, x.RecordId });
                    table.ForeignKey(
                        name: "FK_RecordStyles_Records_RecordId",
                        column: x => x.RecordId,
                        principalTable: "Records",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecordStyles_Styles_StyleId",
                        column: x => x.StyleId,
                        principalTable: "Styles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordStyles_RecordId",
                table: "RecordStyles",
                column: "RecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Languages_LanguageId",
                table: "Records",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
