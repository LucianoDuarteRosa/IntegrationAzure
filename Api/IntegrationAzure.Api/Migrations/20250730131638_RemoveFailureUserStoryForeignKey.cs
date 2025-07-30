using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationAzure.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFailureUserStoryForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_failures_user_stories_UserStoryId",
                table: "failures");

            migrationBuilder.DropIndex(
                name: "IX_failures_UserStoryId",
                table: "failures");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_failures_UserStoryId",
                table: "failures",
                column: "UserStoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_failures_user_stories_UserStoryId",
                table: "failures",
                column: "UserStoryId",
                principalTable: "user_stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
