using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormsWebApplication.Migrations
{
    /// <inheritdoc />
    public partial class Accessor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Visibility",
                table: "Templates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TemplateId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TemplateId",
                table: "AspNetUsers",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Templates_TemplateId",
                table: "AspNetUsers",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Templates_TemplateId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TemplateId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "Templates");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "AspNetUsers");
        }
    }
}
