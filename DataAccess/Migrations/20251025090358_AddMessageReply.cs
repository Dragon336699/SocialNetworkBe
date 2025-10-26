using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageReply : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RepliedMessageId",
                table: "Message",
                type: "uniqueidentifier",
                defaultValue: null,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Message_RepliedMessageId",
                table: "Message",
                column: "RepliedMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Message_RepliedMessageId",
                table: "Message",
                column: "RepliedMessageId",
                principalTable: "Message",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Message_RepliedMessageId",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_RepliedMessageId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "RepliedMessageId",
                table: "Message");
        }
    }
}
