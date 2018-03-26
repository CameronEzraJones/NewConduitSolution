using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Conduit.Migrations
{
    public partial class AddArticles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserIsFollowingDTO_AspNetUsers_FolloweeId",
                table: "UserIsFollowingDTO");

            migrationBuilder.DropForeignKey(
                name: "FK_UserIsFollowingDTO_AspNetUsers_FollowerId",
                table: "UserIsFollowingDTO");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserIsFollowingDTO",
                table: "UserIsFollowingDTO");

            migrationBuilder.RenameTable(
                name: "UserIsFollowingDTO",
                newName: "UserIsFollowing");

            migrationBuilder.RenameIndex(
                name: "IX_UserIsFollowingDTO_FolloweeId",
                table: "UserIsFollowing",
                newName: "IX_UserIsFollowing_FolloweeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserIsFollowing",
                table: "UserIsFollowing",
                columns: new[] { "FollowerId", "FolloweeId" });

            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuthorId = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Slug = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Article_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Tag = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ArticleId = table.Column<int>(nullable: false),
                    AuthorId = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Article_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Article",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comment_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArticleTag",
                columns: table => new
                {
                    TagId = table.Column<int>(nullable: false),
                    ArticleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleTag", x => new { x.TagId, x.ArticleId });
                    table.ForeignKey(
                        name: "FK_ArticleTag_Article_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Article",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleTag_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Article_AuthorId",
                table: "Article",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleTag_ArticleId",
                table: "ArticleTag",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ArticleId",
                table: "Comment",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_AuthorId",
                table: "Comment",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_Tag",
                table: "Tag",
                column: "Tag",
                unique: true,
                filter: "[Tag] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_UserIsFollowing_AspNetUsers_FolloweeId",
                table: "UserIsFollowing",
                column: "FolloweeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserIsFollowing_AspNetUsers_FollowerId",
                table: "UserIsFollowing",
                column: "FollowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserIsFollowing_AspNetUsers_FolloweeId",
                table: "UserIsFollowing");

            migrationBuilder.DropForeignKey(
                name: "FK_UserIsFollowing_AspNetUsers_FollowerId",
                table: "UserIsFollowing");

            migrationBuilder.DropTable(
                name: "ArticleTag");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserIsFollowing",
                table: "UserIsFollowing");

            migrationBuilder.RenameTable(
                name: "UserIsFollowing",
                newName: "UserIsFollowingDTO");

            migrationBuilder.RenameIndex(
                name: "IX_UserIsFollowing_FolloweeId",
                table: "UserIsFollowingDTO",
                newName: "IX_UserIsFollowingDTO_FolloweeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserIsFollowingDTO",
                table: "UserIsFollowingDTO",
                columns: new[] { "FollowerId", "FolloweeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserIsFollowingDTO_AspNetUsers_FolloweeId",
                table: "UserIsFollowingDTO",
                column: "FolloweeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserIsFollowingDTO_AspNetUsers_FollowerId",
                table: "UserIsFollowingDTO",
                column: "FollowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
