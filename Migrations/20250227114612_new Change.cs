using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormsWebApplication.Migrations
{
    /// <inheritdoc />
    public partial class newChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemplateTag_Tags_TagId",
                table: "TemplateTag");

            migrationBuilder.DropForeignKey(
                name: "FK_TemplateTag_Templates_TemplateId",
                table: "TemplateTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TemplateTag",
                table: "TemplateTag");

            migrationBuilder.RenameTable(
                name: "TemplateTag",
                newName: "TemplateTags");

            migrationBuilder.RenameIndex(
                name: "IX_TemplateTag_TagId",
                table: "TemplateTags",
                newName: "IX_TemplateTags_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TemplateTags",
                table: "TemplateTags",
                columns: new[] { "TemplateId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateTags_Tags_TagId",
                table: "TemplateTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateTags_Templates_TemplateId",
                table: "TemplateTags",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemplateTags_Tags_TagId",
                table: "TemplateTags");

            migrationBuilder.DropForeignKey(
                name: "FK_TemplateTags_Templates_TemplateId",
                table: "TemplateTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TemplateTags",
                table: "TemplateTags");

            migrationBuilder.RenameTable(
                name: "TemplateTags",
                newName: "TemplateTag");

            migrationBuilder.RenameIndex(
                name: "IX_TemplateTags_TagId",
                table: "TemplateTag",
                newName: "IX_TemplateTag_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TemplateTag",
                table: "TemplateTag",
                columns: new[] { "TemplateId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateTag_Tags_TagId",
                table: "TemplateTag",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateTag_Templates_TemplateId",
                table: "TemplateTag",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
