using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSearchingHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchingHistory_AspNetUsers_SearchedUserId",
                table: "SearchingHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_SearchingHistory_Group_GroupId",
                table: "SearchingHistory");

            migrationBuilder.DropIndex(
                name: "IX_SearchingHistory_GroupId",
                table: "SearchingHistory");

            migrationBuilder.DropIndex(
                name: "IX_SearchingHistory_SearchedUserId",
                table: "SearchingHistory");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "SearchingHistory");

            migrationBuilder.DropColumn(
                name: "SearchedUserId",
                table: "SearchingHistory");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "SearchingHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NavigateUrl",
                table: "SearchingHistory",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "SearchingHistory");

            migrationBuilder.DropColumn(
                name: "NavigateUrl",
                table: "SearchingHistory");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "SearchingHistory",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SearchedUserId",
                table: "SearchingHistory",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SearchingHistory_GroupId",
                table: "SearchingHistory",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchingHistory_SearchedUserId",
                table: "SearchingHistory",
                column: "SearchedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SearchingHistory_AspNetUsers_SearchedUserId",
                table: "SearchingHistory",
                column: "SearchedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SearchingHistory_Group_GroupId",
                table: "SearchingHistory",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id");
        }
    }
}
