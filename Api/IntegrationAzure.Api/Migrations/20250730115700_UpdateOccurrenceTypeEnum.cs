using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationAzure.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOccurrenceTypeEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_failures_issues_IssueId",
                table: "failures");

            migrationBuilder.DropIndex(
                name: "IX_failures_IssueId",
                table: "failures");

            migrationBuilder.DropColumn(
                name: "IssueId",
                table: "failures");

            migrationBuilder.DropColumn(
                name: "ReportedBy",
                table: "failures");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IssueId",
                table: "failures",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportedBy",
                table: "failures",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_failures_IssueId",
                table: "failures",
                column: "IssueId");

            migrationBuilder.AddForeignKey(
                name: "FK_failures_issues_IssueId",
                table: "failures",
                column: "IssueId",
                principalTable: "issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
