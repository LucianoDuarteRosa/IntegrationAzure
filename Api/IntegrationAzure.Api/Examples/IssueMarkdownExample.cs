using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Application.Services;
using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Examples;

/// <summary>
/// Exemplo de como usar a geração de markdown para Issues
/// </summary>
public static class IssueMarkdownExample
{
    /// <summary>
    /// Demonstra a geração de markdown para uma Issue completa
    /// </summary>
    public static string GetExampleMarkdown()
    {
        var markdownService = new MarkdownGeneratorService();

        var createIssueDto = new CreateIssueDto
        {
            IssueNumber = "ISS-001",
            Title = "Erro na validação de dados do formulário",
            Description = "Issue para demonstrar a geração de markdown",
            Type = IssueType.Bug,
            Priority = Priority.High,
            OccurrenceType = 5, // Erro de Sistema
            Environment = "Production",
            Scenarios = new List<IssueScenarioDto>
            {
                new IssueScenarioDto
                {
                    Given = "Usuário está preenchendo o formulário de cadastro",
                    When = "O usuário clica em salvar com dados válidos",
                    Then = "O sistema deveria aceitar os dados e criar o cadastro"
                },
                new IssueScenarioDto
                {
                    Given = "Formulário está sendo validado",
                    When = "Há um erro de validação do lado servidor",
                    Then = "Uma mensagem de erro clara deveria ser exibida"
                }
            },
            Observations = "Problema ocorre principalmente nos horários de pico",
            Attachments = new List<IssueAttachmentDto>
            {
                new IssueAttachmentDto
                {
                    Name = "screenshot_erro.png",
                    Size = 1024 * 256, // 256 KB
                    Type = "image/png"
                },
                new IssueAttachmentDto
                {
                    Name = "log_aplicacao.txt",
                    Size = 1024 * 50, // 50 KB
                    Type = "text/plain"
                }
            }
        };

        return markdownService.GenerateIssueDescription(createIssueDto);
    }

    /// <summary>
    /// Retorna o markdown de exemplo já formatado
    /// </summary>
    public static string GetFormattedExample()
    {
        return @"## 🎯 Informações da Issue

**📋 Tipo:** 🐛 Bug
**⚡ Prioridade:** 🟠 Alta
**🌐 Ambiente:** Production
**🔧 Tipo de Ocorrência:** Erro de Sistema
---

## Cenários

### Cenário 1

**Dado que:** Usuário está preenchendo o formulário de cadastro

**Quando:** O usuário clica em salvar com dados válidos

**Então:** O sistema deveria aceitar os dados e criar o cadastro

### Cenário 2

**Dado que:** Formulário está sendo validado

**Quando:** Há um erro de validação do lado servidor

**Então:** Uma mensagem de erro clara deveria ser exibida

---

## Observações

- Problema ocorre principalmente nos horários de pico

---

## Anexos

- **screenshot_erro.png** (256.00 KB)
- **log_aplicacao.txt** (50.00 KB)";
    }
}
