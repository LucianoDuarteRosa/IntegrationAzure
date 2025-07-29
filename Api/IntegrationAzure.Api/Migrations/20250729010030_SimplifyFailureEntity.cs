using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationAzure.Api.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyFailureEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_failures_AssignedTo",
                table: "failures");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "failures");

            migrationBuilder.DropColumn(
                name: "DowntimeDuration",
                table: "failures");

            migrationBuilder.DropColumn(
                name: "EstimatedImpactCost",
                table: "failures");

            migrationBuilder.DropColumn(
                name: "ImpactDescription",
                table: "failures");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "failures");

            migrationBuilder.DropColumn(
                name: "PermanentSolution",
                table: "failures");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "failures");

            migrationBuilder.DropColumn(
                name: "RootCauseAnalysis",
                table: "failures");

            migrationBuilder.DropColumn(
                name: "StepsToReproduce",
                table: "failures");

            migrationBuilder.DropColumn(
                name: "SystemsAffected",
                table: "failures");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "failures");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "failures");

            migrationBuilder.DropColumn(
                name: "WorkaroundSolution",
                table: "failures");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "failures",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DowntimeDuration",
                table: "failures",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedImpactCost",
                table: "failures",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImpactDescription",
                table: "failures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "failures",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PermanentSolution",
                table: "failures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                table: "failures",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RootCauseAnalysis",
                table: "failures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StepsToReproduce",
                table: "failures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SystemsAffected",
                table: "failures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "failures",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "failures",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkaroundSolution",
                table: "failures",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_failures_AssignedTo",
                table: "failures",
                column: "AssignedTo");
        }
    }
}
