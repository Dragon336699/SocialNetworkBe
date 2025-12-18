using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullMergeKeyNoti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notification_MergeKey",
                table: "Notification");

            migrationBuilder.AlterColumn<string>(
                name: "MergeKey",
                table: "Notification",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_MergeKey",
                table: "Notification",
                column: "MergeKey",
                unique: true,
                filter: "[MergeKey] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notification_MergeKey",
                table: "Notification");

            migrationBuilder.AlterColumn<string>(
                name: "MergeKey",
                table: "Notification",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_MergeKey",
                table: "Notification",
                column: "MergeKey",
                unique: true);
        }
    }
}
