using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationAzure.Api.Migrations
{
    /// <inheritdoc />
    public partial class ConvertOccurrenceTypeToIntegerCustom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Primeiro, apagar todas as linhas existentes da tabela failures (se houver)
            // porque não sabemos qual valor string foi gravado
            migrationBuilder.Sql("DELETE FROM failures;");

            // Agora alterar o tipo da coluna para integer
            migrationBuilder.AlterColumn<int>(
                name: "OccurrenceType",
                table: "failures",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
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
