using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentMap.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCountryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegionCode",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "RegionName",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "SubregionCode",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "SubregionName",
                table: "Countries");

            migrationBuilder.RenameColumn(
                name: "Boundaries",
                table: "Countries",
                newName: "Shape");

            migrationBuilder.RenameIndex(
                name: "IX_Countries_Boundaries",
                table: "Countries",
                newName: "IX_Countries_Shape");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Countries",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(60)",
                oldMaxLength: 60);

            migrationBuilder.AddColumn<string>(
                name: "LocalName",
                table: "Countries",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalName",
                table: "Countries");

            migrationBuilder.RenameColumn(
                name: "Shape",
                table: "Countries",
                newName: "Boundaries");

            migrationBuilder.RenameIndex(
                name: "IX_Countries_Shape",
                table: "Countries",
                newName: "IX_Countries_Boundaries");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Countries",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<short>(
                name: "RegionCode",
                table: "Countries",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "RegionName",
                table: "Countries",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<short>(
                name: "SubregionCode",
                table: "Countries",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "SubregionName",
                table: "Countries",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");
        }
    }
}
