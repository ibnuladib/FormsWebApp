using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormsWebApplication.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomString1State = table.Column<bool>(type: "bit", nullable: false),
                    CustomString1Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomString2State = table.Column<bool>(type: "bit", nullable: false),
                    CustomString2Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomString3State = table.Column<bool>(type: "bit", nullable: false),
                    CustomString3Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomString4State = table.Column<bool>(type: "bit", nullable: false),
                    CustomString4Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomMultiLine1State = table.Column<bool>(type: "bit", nullable: false),
                    CustomMultiLine1Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomMultiLine2State = table.Column<bool>(type: "bit", nullable: false),
                    CustomMultiLine2Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomMultiLine3State = table.Column<bool>(type: "bit", nullable: false),
                    CustomMultiLine3Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomMultiLine4State = table.Column<bool>(type: "bit", nullable: false),
                    CustomMultiLine4Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomInt1State = table.Column<bool>(type: "bit", nullable: false),
                    CustomInt1Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomInt2State = table.Column<bool>(type: "bit", nullable: false),
                    CustomInt2Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomInt3State = table.Column<bool>(type: "bit", nullable: false),
                    CustomInt3Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomInt4State = table.Column<bool>(type: "bit", nullable: false),
                    CustomInt4Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomCheckbox1State = table.Column<bool>(type: "bit", nullable: false),
                    CustomCheckbox1Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomCheckbox2State = table.Column<bool>(type: "bit", nullable: false),
                    CustomCheckbox2Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomCheckbox3State = table.Column<bool>(type: "bit", nullable: false),
                    CustomCheckbox3Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomCheckbox4State = table.Column<bool>(type: "bit", nullable: false),
                    CustomCheckbox4Question = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Templates_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomString1State = table.Column<bool>(type: "bit", nullable: false),
                    CustomString1Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomString2State = table.Column<bool>(type: "bit", nullable: false),
                    CustomString2Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomString3State = table.Column<bool>(type: "bit", nullable: false),
                    CustomString3Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomString4State = table.Column<bool>(type: "bit", nullable: false),
                    CustomString4Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomMultiLine1State = table.Column<bool>(type: "bit", nullable: false),
                    CustomMultiLine1Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomMultiLine2State = table.Column<bool>(type: "bit", nullable: false),
                    CustomMultiLine2Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomMultiLine3State = table.Column<bool>(type: "bit", nullable: false),
                    CustomMultiLine3Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomMultiLine4State = table.Column<bool>(type: "bit", nullable: false),
                    CustomMultiLine4Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomInt1State = table.Column<bool>(type: "bit", nullable: false),
                    CustomInt1Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomInt2State = table.Column<bool>(type: "bit", nullable: false),
                    CustomInt2Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomInt3State = table.Column<bool>(type: "bit", nullable: false),
                    CustomInt3Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomInt4State = table.Column<bool>(type: "bit", nullable: false),
                    CustomInt4Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomCheckbox1State = table.Column<bool>(type: "bit", nullable: false),
                    CustomCheckbox1Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomCheckbox2State = table.Column<bool>(type: "bit", nullable: false),
                    CustomCheckbox2Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomCheckbox3State = table.Column<bool>(type: "bit", nullable: false),
                    CustomCheckbox3Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomCheckbox4State = table.Column<bool>(type: "bit", nullable: false),
                    CustomCheckbox4Answer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Answers_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Likes_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Likes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TemplateTag",
                columns: table => new
                {
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateTag", x => new { x.TemplateId, x.TagId });
                    table.ForeignKey(
                        name: "FK_TemplateTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TemplateTag_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_TemplateId",
                table: "Answers",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_UserId",
                table: "Answers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ApplicationUserId",
                table: "Comments",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TemplateId",
                table: "Comments",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_ApplicationUserId",
                table: "Likes",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_TemplateId",
                table: "Likes",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId_TemplateId",
                table: "Likes",
                columns: new[] { "UserId", "TemplateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TagName",
                table: "Tags",
                column: "TagName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Templates_AuthorId",
                table: "Templates",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateTag_TagId",
                table: "TemplateTag",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "TemplateTag");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Templates");
        }
    }
}
