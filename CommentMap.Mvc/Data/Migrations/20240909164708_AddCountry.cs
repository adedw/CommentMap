using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace CommentMap.Mvc.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCountry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "Comments",
                type: "geometry (point, 3857)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry (point)");

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    ISO3Code = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    Boundaries = table.Column<MultiPolygon>(type: "geometry (multipolygon, 3857)", nullable: false),
                    ISO2Code = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    Name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    RegionCode = table.Column<short>(type: "smallint", nullable: false),
                    RegionName = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SubregionCode = table.Column<short>(type: "smallint", nullable: false),
                    SubregionName = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.ISO3Code);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_Title",
                table: "Comments",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Boundaries",
                table: "Countries",
                column: "Boundaries")
                .Annotation("Npgsql:IndexMethod", "gist");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Comments_Title",
                table: "Comments");

            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "Comments",
                type: "geometry (point)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry (point, 3857)");
        }
    }
}
