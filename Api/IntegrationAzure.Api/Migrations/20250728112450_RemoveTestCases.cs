using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationAzure.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTestCases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "test_cases");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "test_cases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserStoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    Result = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_cases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_test_cases_user_stories_UserStoryId",
                        column: x => x.UserStoryId,
                        principalTable: "user_stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_test_cases_Status",
                table: "test_cases",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_test_cases_UserStoryId",
                table: "test_cases",
                column: "UserStoryId");
        }
    }
}
