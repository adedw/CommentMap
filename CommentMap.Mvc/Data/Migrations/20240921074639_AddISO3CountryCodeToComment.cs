using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentMap.Mvc.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddISO3CountryCodeToComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ISO3CodeCountry",
                table: "Comments",
                type: "char(3)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ISO3CodeCountry",
                table: "Comments",
                column: "ISO3CodeCountry");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Countries_ISO3CodeCountry",
                table: "Comments",
                column: "ISO3CodeCountry",
                principalTable: "Countries",
                principalColumn: "ISO3Code",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Countries_ISO3CodeCountry",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ISO3CodeCountry",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ISO3CodeCountry",
                table: "Comments");
        }
    }
}
