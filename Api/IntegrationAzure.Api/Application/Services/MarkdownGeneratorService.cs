using System.Text;
using IntegrationAzure.Api.Application.DTOs;

namespace IntegrationAzure.Api.Application.Services;

/// <summary>
/// Serviço responsável por gerar descrições em Markdown para Azure DevOps
/// A partir dos dados estruturados do formulário
/// </summary>
public class MarkdownGeneratorService
{
    /// <summary>
    /// Gera a descrição completa em Markdown para uma história de usuário
    /// </summary>
    public string GenerateUserStoryDescription(CreateUserStoryDto dto)
    {
        var md = new StringBuilder();

        // 1. História do Usuário (Principal)
        GenerateUserStorySection(md, dto.UserStory);

        // 2. Impacto
        GenerateImpactSection(md, dto.Impact);

        // 3. Objetivo
        GenerateObjectiveSection(md, dto.Objective);

        // 4. Telas Ilustrativas
        GenerateScreenshotsSection(md, dto.Screenshots);

        // 5. Campos de Preenchimento
        GenerateFormFieldsSection(md, dto.FormFields);

        // 6. Mensagens Informativas
        GenerateMessagesSection(md, dto.Messages);

        // 7. Regras de Negócio
        GenerateBusinessRulesSection(md, dto.BusinessRules);

        // 8. Cenários
        GenerateScenariosSection(md, dto.Scenarios);

        // 9. Anexos
        GenerateAttachmentsSection(md, dto.Attachments);

        return md.ToString().Trim();
    }

    private void GenerateUserStorySection(StringBuilder md, UserStoryStructureDto? userStory)
    {
        if (userStory == null) return;

        md.AppendLine("# História do Usuário");
        md.AppendLine();

        if (!string.IsNullOrWhiteSpace(userStory.Como))
        {
            md.AppendLine($"**Como:** {userStory.Como}");
            md.AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(userStory.Quero))
        {
            md.AppendLine($"**Quero:** {userStory.Quero}");
            md.AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(userStory.Para))
        {
            md.AppendLine($"**Para:** {userStory.Para}");
            md.AppendLine();
        }

        md.AppendLine("---");
        md.AppendLine();
    }

    private void GenerateImpactSection(StringBuilder md, ImpactSectionDto? impact)
    {
        if (impact?.Items == null || !impact.Items.Any()) return;

        md.AppendLine("## Impacto");
        md.AppendLine();

        for (int i = 0; i < impact.Items.Count; i++)
        {
            var item = impact.Items[i];
            md.AppendLine($"### Impacto {i + 1}");
            md.AppendLine();

            if (!string.IsNullOrWhiteSpace(item.Current))
            {
                md.AppendLine("**Processo Atual:**");
                md.AppendLine(item.Current);
                md.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(item.Expected))
            {
                md.AppendLine("**Melhoria Esperada:**");
                md.AppendLine(item.Expected);
                md.AppendLine();
            }
        }

        md.AppendLine("---");
        md.AppendLine();
    }

    private void GenerateObjectiveSection(StringBuilder md, ObjectiveSectionDto? objective)
    {
        if (objective?.Fields == null || !objective.Fields.Any()) return;

        md.AppendLine("## Objetivo");
        md.AppendLine();

        foreach (var field in objective.Fields)
        {
            if (!string.IsNullOrWhiteSpace(field.Content))
            {
                md.AppendLine($"- {field.Content}");
            }
        }

        md.AppendLine();
        md.AppendLine("---");
        md.AppendLine();
    }

    private void GenerateScreenshotsSection(StringBuilder md, ScreenshotsSectionDto? screenshots)
    {
        if (screenshots?.Items == null || !screenshots.Items.Any()) return;

        md.AppendLine("## Telas Ilustrativas");
        md.AppendLine();

        md.AppendLine("| Arquivo | Tamanho | Tipo |");
        md.AppendLine("|---------|---------|------|");

        foreach (var item in screenshots.Items)
        {
            var sizeFormatted = FormatFileSize(item.Size);
            md.AppendLine($"| {item.Name} | {sizeFormatted} | {item.Type} |");
        }

        md.AppendLine();
        md.AppendLine("*As imagens serão anexadas a esta história.*");
        md.AppendLine();
        md.AppendLine("---");
        md.AppendLine();
    }

    private void GenerateFormFieldsSection(StringBuilder md, FormFieldsSectionDto? formFields)
    {
        if (formFields?.Items == null || !formFields.Items.Any()) return;

        md.AppendLine("## Campos de Preenchimento");
        md.AppendLine();

        md.AppendLine("| Nome do Campo | Tipo | Tamanho Máximo | Obrigatório |");
        md.AppendLine("|---------------|------|----------------|-------------|");

        foreach (var field in formFields.Items)
        {
            var typeFormatted = FormatFieldType(field.Type);
            var sizeDisplay = string.IsNullOrWhiteSpace(field.Size) ? "-" : field.Size;
            var requiredDisplay = field.Required ? "Sim" : "Não";

            md.AppendLine($"| {field.Name} | {typeFormatted} | {sizeDisplay} | {requiredDisplay} |");
        }

        md.AppendLine();
        md.AppendLine("---");
        md.AppendLine();
    }

    private void GenerateMessagesSection(StringBuilder md, MessagesSectionDto? messages)
    {
        if (messages?.Items == null || !messages.Items.Any()) return;

        md.AppendLine("## Mensagens Informativas");
        md.AppendLine();

        foreach (var message in messages.Items)
        {
            if (!string.IsNullOrWhiteSpace(message.Content))
            {
                md.AppendLine($"- {message.Content}");
            }
        }

        md.AppendLine();
        md.AppendLine("---");
        md.AppendLine();
    }

    private void GenerateBusinessRulesSection(StringBuilder md, BusinessRulesSectionDto? businessRules)
    {
        if (businessRules?.Items == null || !businessRules.Items.Any()) return;

        md.AppendLine("## Regras de Negócio");
        md.AppendLine();

        for (int i = 0; i < businessRules.Items.Count; i++)
        {
            var rule = businessRules.Items[i];
            if (!string.IsNullOrWhiteSpace(rule.Content))
            {
                md.AppendLine($"{i + 1}. {rule.Content}");
            }
        }

        md.AppendLine();
        md.AppendLine("---");
        md.AppendLine();
    }

    private void GenerateScenariosSection(StringBuilder md, ScenariosSectionDto? scenarios)
    {
        if (scenarios?.Items == null || !scenarios.Items.Any()) return;

        md.AppendLine("## Cenários");
        md.AppendLine();

        for (int i = 0; i < scenarios.Items.Count; i++)
        {
            var scenario = scenarios.Items[i];
            md.AppendLine($"### Cenário {i + 1}");
            md.AppendLine();

            if (!string.IsNullOrWhiteSpace(scenario.Given))
            {
                md.AppendLine($"**Dado que:** {scenario.Given}");
                md.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(scenario.When))
            {
                md.AppendLine($"**Quando:** {scenario.When}");
                md.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(scenario.Then))
            {
                md.AppendLine($"**Então:** {scenario.Then}");
                md.AppendLine();
            }
        }

        md.AppendLine("---");
        md.AppendLine();
    }

    private void GenerateAttachmentsSection(StringBuilder md, AttachmentsSectionDto? attachments)
    {
        if (attachments?.Items == null || !attachments.Items.Any()) return;

        md.AppendLine("## Anexos");
        md.AppendLine();

        md.AppendLine("| Arquivo | Tamanho | Tipo |");
        md.AppendLine("|---------|---------|------|");

        foreach (var item in attachments.Items)
        {
            var sizeFormatted = FormatFileSize(item.Size);
            md.AppendLine($"| {item.Name} | {sizeFormatted} | {item.Type} |");
        }

        md.AppendLine();
        md.AppendLine("*Os arquivos serão anexados a esta história.*");
        md.AppendLine();
    }

    private string FormatFieldType(string type)
    {
        return type switch
        {
            "text" => "Texto",
            "number" => "Número",
            "date" => "Data",
            "datetime" => "Data e Hora",
            "boolean" => "Sim/Não",
            "select" => "Lista de Opções",
            _ => type
        };
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }
}
