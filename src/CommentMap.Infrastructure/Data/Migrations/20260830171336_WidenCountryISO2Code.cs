using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentMap.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class WidenCountryISO2Code : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ISO2Code",
                table: "Countries",
                type: "char(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(2)",
                oldMaxLength: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ISO2Code",
                table: "Countries",
                type: "char(2)",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(3)",
                oldMaxLength: 3);
        }
    }
}
