using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationAzure.Api.Migrations
{
    /// <inheritdoc />
    public partial class ConvertOccurrenceTypeWithUsing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Limpeza da tabela failures antes da conversão
            migrationBuilder.Sql("DELETE FROM failures;");

            // Usar SQL direto para conversão de tipo com USING
            migrationBuilder.Sql(@"
                ALTER TABLE failures 
                ALTER COLUMN ""OccurrenceType"" 
                TYPE integer 
                USING CASE 
                    WHEN ""OccurrenceType"" IS NULL THEN 0
                    ELSE 0
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OccurrenceType",
                table: "failures",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
