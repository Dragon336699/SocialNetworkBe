using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationData");

            migrationBuilder.DropTable(
                name: "NotificationUser");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Notification");

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MergeKey",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NavigateUrl",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Unread",
                table: "Notification",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "MergeKey",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "NavigateUrl",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "Unread",
                table: "Notification");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Notification",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "NotificationData",
                columns: table => new
                {
                    ActorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationData", x => new { x.ActorId, x.NotificationId });
                    table.ForeignKey(
                        name: "FK_NotificationData_AspNetUsers_ActorId",
                        column: x => x.ActorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationData_Comment_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificationData_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificationData_Notification_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationData_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NotificationUser",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsMuted = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationUser", x => new { x.UserId, x.NotificationId });
                    table.ForeignKey(
                        name: "FK_NotificationUser_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationUser_Notification_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationData_CommentId",
                table: "NotificationData",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationData_GroupId",
                table: "NotificationData",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationData_NotificationId",
                table: "NotificationData",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationData_PostId",
                table: "NotificationData",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationUser_NotificationId",
                table: "NotificationUser",
                column: "NotificationId");
        }
    }
}
