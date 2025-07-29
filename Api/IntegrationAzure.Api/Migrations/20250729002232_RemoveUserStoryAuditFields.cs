using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationAzure.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserStoryAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "user_stories");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "user_stories");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "user_stories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "user_stories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "user_stories",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "user_stories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
