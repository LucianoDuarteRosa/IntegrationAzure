using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Application.Services;
using IntegrationAzure.Api.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationAzure.Api.Controllers;

/// <summary>
/// Controller para testes da geração de markdown
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly MarkdownGeneratorService _markdownGeneratorService;

    public TestController(MarkdownGeneratorService markdownGeneratorService)
    {
        _markdownGeneratorService = markdownGeneratorService;
    }

    /// <summary>
    /// Testa a geração de markdown para Issues
    /// </summary>
    [HttpPost("issue-markdown")]
    public ActionResult<string> TestIssueMarkdown([FromBody] CreateIssueDto dto)
    {
        try
        {
            // Log para debug
            Console.WriteLine($"Recebido DTO - Title: {dto.Title}");
            Console.WriteLine($"Scenarios count: {dto.Scenarios?.Count ?? 0}");
            Console.WriteLine($"Observations: '{dto.Observations ?? "null"}'");
            Console.WriteLine($"Attachments count: {dto.Attachments?.Count ?? 0}");

            if (dto.Scenarios != null)
            {
                for (int i = 0; i < dto.Scenarios.Count; i++)
                {
                    var scenario = dto.Scenarios[i];
                    Console.WriteLine($"Scenario {i + 1}: Given='{scenario.Given}', When='{scenario.When}', Then='{scenario.Then}'");
                }
            }

            var markdown = _markdownGeneratorService.GenerateIssueDescription(dto);

            Console.WriteLine("Markdown gerado:");
            Console.WriteLine(markdown);

            return Ok(markdown);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro: {ex.Message}");
        }
    }

    /// <summary>
    /// Retorna um exemplo de CreateIssueDto para teste
    /// </summary>
    [HttpGet("issue-example")]
    public ActionResult<CreateIssueDto> GetIssueExample()
    {
        return Ok(new CreateIssueDto
        {
            IssueNumber = "ISS-TEST-001",
            Title = "Teste de geração de markdown",
            Description = "Issue de teste",
            Type = IssueType.Bug,
            Priority = Priority.High,
            OccurrenceType = 5,
            Environment = "Test",
            Scenarios = new List<IssueScenarioDto>
            {
                new IssueScenarioDto
                {
                    Given = "Usuário está na tela principal",
                    When = "Clica no botão salvar",
                    Then = "Deveria salvar os dados com sucesso"
                },
                new IssueScenarioDto
                {
                    Given = "Sistema está processando dados",
                    When = "Ocorre um erro de validação",
                    Then = "Deveria exibir mensagem de erro clara"
                }
            },
            Observations = "Esta é uma observação de teste para verificar se está funcionando corretamente",
            Attachments = new List<IssueAttachmentDto>
            {
                new IssueAttachmentDto
                {
                    Name = "teste.png",
                    Size = 1024 * 100, // 100 KB
                    Type = "image/png"
                }
            }
        });
    }
}
