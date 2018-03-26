using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Conduit.Migrations
{
    public partial class AddUserIsFollowingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserIsFollowingDTO",
                columns: table => new
                {
                    FollowerId = table.Column<string>(nullable: false),
                    FolloweeId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIsFollowingDTO", x => new { x.FollowerId, x.FolloweeId });
                    table.ForeignKey(
                        name: "FK_UserIsFollowingDTO_AspNetUsers_FolloweeId",
                        column: x => x.FolloweeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_UserIsFollowingDTO_AspNetUsers_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserIsFollowingDTO_FolloweeId",
                table: "UserIsFollowingDTO",
                column: "FolloweeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserIsFollowingDTO");
        }
    }
}
