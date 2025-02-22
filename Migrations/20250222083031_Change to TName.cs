using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormsWebApplication.Migrations
{
    /// <inheritdoc />
    public partial class ChangetoTName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TagName",
                table: "Tags",
                newName: "TName");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_TagName",
                table: "Tags",
                newName: "IX_Tags_TName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TName",
                table: "Tags",
                newName: "TagName");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_TName",
                table: "Tags",
                newName: "IX_Tags_TagName");
        }
    }
}
