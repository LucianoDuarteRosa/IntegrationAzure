using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationAzure.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_stories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DemandNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AcceptanceCriteria = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_stories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "issues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IssueNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AssignedTo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Reporter = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Environment = table.Column<string>(type: "text", nullable: true),
                    StepsToReproduce = table.Column<string>(type: "text", nullable: true),
                    ExpectedResult = table.Column<string>(type: "text", nullable: true),
                    ActualResult = table.Column<string>(type: "text", nullable: true),
                    UserStoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Resolution = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_issues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_issues_user_stories_UserStoryId",
                        column: x => x.UserStoryId,
                        principalTable: "user_stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "test_cases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    UserStoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Result = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "failures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FailureNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Severity = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReportedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AssignedTo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Environment = table.Column<string>(type: "text", nullable: true),
                    SystemsAffected = table.Column<string>(type: "text", nullable: true),
                    ImpactDescription = table.Column<string>(type: "text", nullable: true),
                    StepsToReproduce = table.Column<string>(type: "text", nullable: true),
                    WorkaroundSolution = table.Column<string>(type: "text", nullable: true),
                    RootCauseAnalysis = table.Column<string>(type: "text", nullable: true),
                    PermanentSolution = table.Column<string>(type: "text", nullable: true),
                    IssueId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserStoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DowntimeDuration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    EstimatedImpactCost = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_failures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_failures_issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_failures_user_stories_UserStoryId",
                        column: x => x.UserStoryId,
                        principalTable: "user_stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "attachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    UserStoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    IssueId = table.Column<Guid>(type: "uuid", nullable: true),
                    FailureId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_attachments_failures_FailureId",
                        column: x => x.FailureId,
                        principalTable: "failures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attachments_issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "issues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attachments_user_stories_UserStoryId",
                        column: x => x.UserStoryId,
                        principalTable: "user_stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_attachments_FailureId",
                table: "attachments",
                column: "FailureId");

            migrationBuilder.CreateIndex(
                name: "IX_attachments_IssueId",
                table: "attachments",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_attachments_UserStoryId",
                table: "attachments",
                column: "UserStoryId");

            migrationBuilder.CreateIndex(
                name: "IX_failures_AssignedTo",
                table: "failures",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_failures_FailureNumber",
                table: "failures",
                column: "FailureNumber");

            migrationBuilder.CreateIndex(
                name: "IX_failures_IssueId",
                table: "failures",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_failures_OccurredAt",
                table: "failures",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_failures_Severity",
                table: "failures",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_failures_Status",
                table: "failures",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_failures_UserStoryId",
                table: "failures",
                column: "UserStoryId");

            migrationBuilder.CreateIndex(
                name: "IX_issues_AssignedTo",
                table: "issues",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_issues_IssueNumber",
                table: "issues",
                column: "IssueNumber");

            migrationBuilder.CreateIndex(
                name: "IX_issues_Priority",
                table: "issues",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_issues_Status",
                table: "issues",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_issues_Type",
                table: "issues",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_issues_UserStoryId",
                table: "issues",
                column: "UserStoryId");

            migrationBuilder.CreateIndex(
                name: "IX_test_cases_Status",
                table: "test_cases",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_test_cases_UserStoryId",
                table: "test_cases",
                column: "UserStoryId");

            migrationBuilder.CreateIndex(
                name: "IX_user_stories_CreatedAt",
                table: "user_stories",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_user_stories_DemandNumber",
                table: "user_stories",
                column: "DemandNumber");

            migrationBuilder.CreateIndex(
                name: "IX_user_stories_Priority",
                table: "user_stories",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_user_stories_Status",
                table: "user_stories",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attachments");

            migrationBuilder.DropTable(
                name: "test_cases");

            migrationBuilder.DropTable(
                name: "failures");

            migrationBuilder.DropTable(
                name: "issues");

            migrationBuilder.DropTable(
                name: "user_stories");
        }
    }
}
