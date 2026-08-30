using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentMap.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFilteredIsDeletedIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Comments_IsDeleted_Filtered",
                table: "Comments",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comments_IsDeleted_Filtered",
                table: "Comments");
        }
    }
}
