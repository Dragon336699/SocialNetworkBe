using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentReactionUser_Reaction_ReactionId",
                table: "CommentReactionUser");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageReactionUser_Reaction_ReactionId",
                table: "MessageReactionUser");

            migrationBuilder.DropForeignKey(
                name: "FK_PostReactionUser_Reaction_ReactionId",
                table: "PostReactionUser");

            migrationBuilder.DropTable(
                name: "Reaction");

            migrationBuilder.DropIndex(
                name: "IX_PostReactionUser_ReactionId",
                table: "PostReactionUser");

            migrationBuilder.DropIndex(
                name: "IX_MessageReactionUser_ReactionId",
                table: "MessageReactionUser");

            migrationBuilder.DropIndex(
                name: "IX_CommentReactionUser_ReactionId",
                table: "CommentReactionUser");

            migrationBuilder.DropColumn(
                name: "ReactionId",
                table: "PostReactionUser");

            migrationBuilder.DropColumn(
                name: "ReactionId",
                table: "MessageReactionUser");

            migrationBuilder.DropColumn(
                name: "ReactionId",
                table: "CommentReactionUser");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PostReactionUser",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "PostReactionUser",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "MessageReactionUser",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Reaction",
                table: "MessageReactionUser",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "MessageReactionUser",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CommentReactionUser",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CommentReactionUser",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PostReactionUser");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PostReactionUser");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "MessageReactionUser");

            migrationBuilder.DropColumn(
                name: "Reaction",
                table: "MessageReactionUser");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MessageReactionUser");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CommentReactionUser");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CommentReactionUser");

            migrationBuilder.AddColumn<Guid>(
                name: "ReactionId",
                table: "PostReactionUser",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ReactionId",
                table: "MessageReactionUser",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ReactionId",
                table: "CommentReactionUser",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Reaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    IconSymbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reaction", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostReactionUser_ReactionId",
                table: "PostReactionUser",
                column: "ReactionId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReactionUser_ReactionId",
                table: "MessageReactionUser",
                column: "ReactionId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactionUser_ReactionId",
                table: "CommentReactionUser",
                column: "ReactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReactionUser_Reaction_ReactionId",
                table: "CommentReactionUser",
                column: "ReactionId",
                principalTable: "Reaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReactionUser_Reaction_ReactionId",
                table: "MessageReactionUser",
                column: "ReactionId",
                principalTable: "Reaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostReactionUser_Reaction_ReactionId",
                table: "PostReactionUser",
                column: "ReactionId",
                principalTable: "Reaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
