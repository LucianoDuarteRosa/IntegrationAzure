using System.Text;
using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Application.Services;

/// <summary>
/// Serviço responsável por gerar descrições em HTML para Azure DevOps
/// A partir dos dados estruturados do formulário
/// </summary>
public class HtmlGeneratorService
{
    /// <summary>
    /// Gera a descrição completa em HTML para uma história de usuário
    /// </summary>
    public string GenerateUserStoryDescription(CreateUserStoryDto dto)
    {
        return GenerateUserStoryDescription(dto, includeAcceptanceCriteria: true);
    }

    /// <summary>
    /// Gera a descrição completa em HTML para uma história de usuário
    /// </summary>
    /// <param name="dto">DTO da história de usuário</param>
    /// <param name="includeAcceptanceCriteria">Se deve incluir os critérios de aceite no HTML</param>
    public string GenerateUserStoryDescription(CreateUserStoryDto dto, bool includeAcceptanceCriteria)
    {
        var html = new StringBuilder();

        // 1. História do Usuário (Principal)
        GenerateUserStorySection(html, dto.UserStory);

        // 2. Critérios de Aceite (apenas se solicitado)
        if (includeAcceptanceCriteria)
        {
            GenerateAcceptanceCriteriaSection(html, dto.AcceptanceCriteria);
        }

        // 3. Impacto
        GenerateImpactSection(html, dto.Impact);

        // 4. Objetivo
        GenerateObjectiveSection(html, dto.Objective);

        // 5. Telas Ilustrativas
        GenerateScreenshotsSection(html, dto.Screenshots);

        // 6. Campos de Preenchimento
        GenerateFormFieldsSection(html, dto.FormFields);

        // 7. Mensagens Informativas
        GenerateMessagesSection(html, dto.Messages);

        // 8. Regras de Negócio
        GenerateBusinessRulesSection(html, dto.BusinessRules);

        // 9. Cenários
        GenerateScenariosSection(html, dto.Scenarios);

        // 10. Anexos
        GenerateAttachmentsSection(html, dto.Attachments);

        return html.ToString().Trim();
    }

    private void GenerateUserStorySection(StringBuilder html, UserStoryStructureDto? userStory)
    {
        if (userStory == null) return;

        html.AppendLine("<h1>📖 História do Usuário</h1>");

        if (!string.IsNullOrWhiteSpace(userStory.Como))
        {
            html.AppendLine($"<p><strong>Como:</strong> {userStory.Como}</p>");
        }

        if (!string.IsNullOrWhiteSpace(userStory.Quero))
        {
            html.AppendLine($"<p><strong>Quero:</strong> {userStory.Quero}</p>");
        }

        if (!string.IsNullOrWhiteSpace(userStory.Para))
        {
            html.AppendLine($"<p><strong>Para:</strong> {userStory.Para}</p>");
        }
        html.AppendLine("</br>");
        html.AppendLine("<hr/>");
        html.AppendLine("</br>");
    }

    private void GenerateAcceptanceCriteriaSection(StringBuilder html, string? acceptanceCriteria)
    {
        if (string.IsNullOrWhiteSpace(acceptanceCriteria)) return;

        html.AppendLine("<h1>✅ Critérios de Aceite</h1>");

        // Dividir os critérios por linha e formatar como lista
        var criteria = acceptanceCriteria.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        if (criteria.Length > 1)
        {
            html.AppendLine("<ul>");
            foreach (var criterion in criteria)
            {
                var trimmed = criterion.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    // Remove possível marcador de lista existente (-, *, •)
                    if (trimmed.StartsWith("- ") || trimmed.StartsWith("* ") || trimmed.StartsWith("• "))
                    {
                        trimmed = trimmed.Substring(2).Trim();
                    }
                    html.AppendLine($"<li>{trimmed}</li>");
                }
            }
            html.AppendLine("</ul>");
        }
        else
        {
            // Se for apenas um critério, mostrar como parágrafo
            html.AppendLine($"<p>{acceptanceCriteria}</p>");
        }
        html.AppendLine("</br>");
        html.AppendLine("<hr/>");
        html.AppendLine("</br>");
    }

    private void GenerateImpactSection(StringBuilder html, ImpactSectionDto? impact)
    {
        if (impact?.Items == null || !impact.Items.Any()) return;

        html.AppendLine("<h1>Impacto</h1>");
        html.AppendLine("</br>");
        for (int i = 0; i < impact.Items.Count; i++)
        {
            var item = impact.Items[i];
            html.AppendLine($"<h2>Impacto {i + 1}</h2>");

            if (!string.IsNullOrWhiteSpace(item.Current))
            {
                html.AppendLine("<p><strong>Processo Atual:</strong></p>");
                html.AppendLine($"<p>{item.Current}</p>");
            }

            if (!string.IsNullOrWhiteSpace(item.Expected))
            {
                html.AppendLine("<p><strong>Melhoria Esperada:</strong></p>");
                html.AppendLine($"<p>{item.Expected}</p>");
            }
            html.AppendLine("</br>");
        }
        html.AppendLine("<hr/>");
        html.AppendLine("</br>");
    }

    private void GenerateObjectiveSection(StringBuilder html, ObjectiveSectionDto? objective)
    {
        if (objective?.Fields == null || !objective.Fields.Any()) return;

        html.AppendLine("<h1>Objetivo</h1>");
        html.AppendLine("<ul>");

        foreach (var field in objective.Fields)
        {
            if (!string.IsNullOrWhiteSpace(field.Content))
            {
                html.AppendLine($"<li>{field.Content}</li>");
            }
        }

        html.AppendLine("</ul>");
        html.AppendLine("</br>");
        html.AppendLine("<hr/>");
        html.AppendLine("</br>");
    }

    private void GenerateScreenshotsSection(StringBuilder html, ScreenshotsSectionDto? screenshots)
    {
        if (screenshots?.Items == null || !screenshots.Items.Any()) return;

        html.AppendLine("<h1>Telas Ilustrativas</h1>");
        html.AppendLine("<table border='1'>");
        html.AppendLine("<tr><th>Arquivo</th><th>Tamanho</th><th>Tipo</th></tr>");

        foreach (var item in screenshots.Items)
        {
            var sizeFormatted = FormatFileSize(item.Size);
            html.AppendLine($"<tr><td>{item.Name}</td><td>{sizeFormatted}</td><td>{item.Type}</td></tr>");
        }

        html.AppendLine("</table>");
        html.AppendLine("<p><em>As imagens serão anexadas a esta história.</em></p>");
        html.AppendLine("</br>");
        html.AppendLine("<hr/>");
        html.AppendLine("</br>");
    }

    private void GenerateFormFieldsSection(StringBuilder html, FormFieldsSectionDto? formFields)
    {
        if (formFields?.Items == null || !formFields.Items.Any()) return;

        html.AppendLine("<h1>Campos de Preenchimento</h1>");
        html.AppendLine("<table border='1'>");
        html.AppendLine("<tr><th>Nome do Campo</th><th>Tipo</th><th>Tamanho Máximo</th><th>Obrigatório</th></tr>");

        foreach (var field in formFields.Items)
        {
            var typeFormatted = FormatFieldType(field.Type);
            var sizeDisplay = string.IsNullOrWhiteSpace(field.Size) ? "-" : field.Size;
            var requiredDisplay = field.Required ? "Sim" : "Não";

            html.AppendLine($"<tr><td>{field.Name}</td><td>{typeFormatted}</td><td>{sizeDisplay}</td><td>{requiredDisplay}</td></tr>");
        }

        html.AppendLine("</table>");
        html.AppendLine("</br>");
        html.AppendLine("<hr/>");
        html.AppendLine("</br>");
    }

    private void GenerateMessagesSection(StringBuilder html, MessagesSectionDto? messages)
    {
        if (messages?.Items == null || !messages.Items.Any()) return;

        html.AppendLine("<h1>Mensagens Informativas</h1>");
        html.AppendLine("<ul>");

        foreach (var message in messages.Items)
        {
            if (!string.IsNullOrWhiteSpace(message.Content))
            {
                html.AppendLine($"<li>{message.Content}</li>");
            }
        }

        html.AppendLine("</ul>");
        html.AppendLine("</br>");
        html.AppendLine("<hr/>");
        html.AppendLine("</br>");
    }

    private void GenerateBusinessRulesSection(StringBuilder html, BusinessRulesSectionDto? businessRules)
    {
        if (businessRules?.Items == null || !businessRules.Items.Any()) return;

        html.AppendLine("<h1>Regras de Negócio</h1>");
        html.AppendLine("<ol>");

        foreach (var rule in businessRules.Items)
        {
            if (!string.IsNullOrWhiteSpace(rule.Content))
            {
                html.AppendLine($"<li>{rule.Content}</li>");
            }
        }

        html.AppendLine("</ol>");
        html.AppendLine("</br>");
        html.AppendLine("<hr/>");
        html.AppendLine("</br>");
    }

    private void GenerateScenariosSection(StringBuilder html, ScenariosSectionDto? scenarios)
    {
        if (scenarios?.Items == null || !scenarios.Items.Any()) return;

        html.AppendLine("<h1>Cenários</h1>");
        html.AppendLine("</br>");
        for (int i = 0; i < scenarios.Items.Count; i++)
        {
            var scenario = scenarios.Items[i];
            html.AppendLine($"<h2>Cenário {i + 1}</h2>");

            if (!string.IsNullOrWhiteSpace(scenario.Given))
            {
                html.AppendLine($"<p><strong>Dado que:</strong> {scenario.Given}</p>");
            }

            if (!string.IsNullOrWhiteSpace(scenario.When))
            {
                html.AppendLine($"<p><strong>Quando:</strong> {scenario.When}</p>");
            }

            if (!string.IsNullOrWhiteSpace(scenario.Then))
            {
                html.AppendLine($"<p><strong>Então:</strong> {scenario.Then}</p>");
            }
            html.AppendLine("</br>");
        }
        html.AppendLine("<hr/>");
        html.AppendLine("</br>");
    }

    private void GenerateAttachmentsSection(StringBuilder html, AttachmentsSectionDto? attachments)
    {
        if (attachments?.Items == null || !attachments.Items.Any()) return;

        html.AppendLine("<h1>Anexos</h1>");
        html.AppendLine("<table border='1'>");
        html.AppendLine("<tr><th>Arquivo</th><th>Tamanho</th><th>Tipo</th></tr>");

        foreach (var item in attachments.Items)
        {
            var sizeFormatted = FormatFileSize(item.Size);
            html.AppendLine($"<tr><td>{item.Name}</td><td>{sizeFormatted}</td><td>{item.Type}</td></tr>");
        }

        html.AppendLine("</table>");
        html.AppendLine("<p><em>Os arquivos serão anexados a esta história.</em></p>");
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
            "button" => "Botão",
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

    /// <summary>
    /// Gera a descrição completa em HTML para uma falha
    /// </summary>
    public string GenerateFailureDescription(CreateFailureDto dto, List<FailureScenarioDto>? scenarios = null, string? observations = null)
    {
        var html = new StringBuilder();

        // 1. Informações Básicas
        html.AppendLine("<h2>🐛 Informações da Falha</h2>");
        html.AppendLine($"<p><strong>🌐 Ambiente:</strong> {dto.Environment}</p>");
        html.AppendLine($"<p><strong>⚠️ Severidade:</strong> {GetSeverityText(dto.Severity)}</p>");
        html.AppendLine("<hr/>");

        // 2. Impactos da Falha
        if (scenarios != null && scenarios.Any())
        {
            html.AppendLine("<h2>Impacto</h2>");

            for (int i = 0; i < scenarios.Count; i++)
            {
                var scenario = scenarios[i];
                html.AppendLine($"<h3>Impacto {i + 1}</h3>");
                html.AppendLine("<p><strong>Processo Atual:</strong></p>");
                html.AppendLine($"<p>{scenario.Given}</p>");
                html.AppendLine("<p><strong>Melhoria Esperada:</strong></p>");
                html.AppendLine($"<p>{scenario.Then}</p>");
            }

            html.AppendLine("<hr/>");
            html.AppendLine("</br>");
        }

        // 3. Observações
        if (!string.IsNullOrEmpty(observations))
        {
            html.AppendLine("<h2>Observação</h2>");
            html.AppendLine($"<p>{observations}</p>");
            html.AppendLine("<hr/>");
            html.AppendLine("</br>");
        }

        // 4. Evidências
        if (dto.Attachments != null && dto.Attachments.Any())
        {
            html.AppendLine("<h2>Evidências</h2>");
            html.AppendLine("<ul>");

            foreach (var attachment in dto.Attachments)
            {
                var sizeFormatted = FormatFileSize(attachment.Size);
                html.AppendLine($"<li><strong>{attachment.Name}</strong> ({sizeFormatted})</li>");
            }

            html.AppendLine("</ul>");
        }

        return html.ToString().Trim();
    }

    /// <summary>
    /// Gera a descrição completa em HTML para uma issue
    /// </summary>
    public string GenerateIssueDescription(CreateIssueDto dto, string? observations = null)
    {
        var html = new StringBuilder();

        // 1. Informações Básicas
        html.AppendLine("<h1>🎯 Informações da Issue</h1>");
        html.AppendLine($"<p><strong>📋 Tipo:</strong> {GetIssueTypeText(dto.Type)}</p>");
        html.AppendLine($"<p><strong>⚡ Prioridade:</strong> {GetPriorityText(dto.Priority)}</p>");
        html.AppendLine($"<p><strong>🌐 Ambiente:</strong> {dto.Environment ?? "Não especificado"}</p>");
        html.AppendLine($"<p><strong>🔧 Tipo de Ocorrência:</strong> {GetOccurrenceTypeText(dto.OccurrenceType)}</p>");
        html.AppendLine("<hr/>");
        html.AppendLine("</br>");

        // 2. Cenários da Issue
        if (dto.Scenarios != null && dto.Scenarios.Any())
        {
            html.AppendLine("<h1>Cenários</h1>");

            for (int i = 0; i < dto.Scenarios.Count; i++)
            {
                var scenario = dto.Scenarios[i];
                html.AppendLine($"<h2>Cenário {i + 1}</h2>");

                if (!string.IsNullOrWhiteSpace(scenario.Given))
                {
                    html.AppendLine($"<p><strong>Dado que:</strong> {scenario.Given}</p>");
                }

                if (!string.IsNullOrWhiteSpace(scenario.When))
                {
                    html.AppendLine($"<p><strong>Quando:</strong> {scenario.When}</p>");
                }

                if (!string.IsNullOrWhiteSpace(scenario.Then))
                {
                    html.AppendLine($"<p><strong>Então:</strong> {scenario.Then}</p>");
                }
            }

            html.AppendLine("</br>");
            html.AppendLine("<hr/>");
            html.AppendLine("</br>");
        }

        // 3. Observações
        if (!string.IsNullOrEmpty(observations) || !string.IsNullOrEmpty(dto.Observations))
        {
            html.AppendLine("<h1>Observações</h1>");
            html.AppendLine("<ul>");

            if (!string.IsNullOrEmpty(observations))
            {
                html.AppendLine($"<li>{observations}</li>");
            }

            if (!string.IsNullOrEmpty(dto.Observations))
            {
                html.AppendLine($"<li>{dto.Observations}</li>");
            }

            html.AppendLine("</ul>");
            html.AppendLine("</br>");
            html.AppendLine("<hr/>");
            html.AppendLine("</br>");
        }

        // 4. Anexos
        if (dto.Attachments != null && dto.Attachments.Any())
        {
            html.AppendLine("<h1>Anexos</h1>");
            html.AppendLine("<ul>");

            foreach (var attachment in dto.Attachments)
            {
                var sizeFormatted = FormatFileSize(attachment.Size);
                html.AppendLine($"<li><strong>{attachment.Name}</strong> ({sizeFormatted})</li>");
            }

            html.AppendLine("</ul>");
        }

        return html.ToString().Trim();
    }

    private string GetSeverityText(FailureSeverity severity)
    {
        return severity switch
        {
            FailureSeverity.Low => "🟢 Baixa",
            FailureSeverity.Medium => "🟡 Média",
            FailureSeverity.High => "🟠 Alta",
            FailureSeverity.Critical => "🔴 Crítica",
            _ => "❓ Não especificada"
        };
    }

    private string GetIssueTypeText(IssueType type)
    {
        return type switch
        {
            IssueType.Bug => "🐛 Bug",
            IssueType.Feature => "✨ Nova Funcionalidade",
            IssueType.Improvement => "🚀 Melhoria",
            IssueType.Task => "📋 Tarefa",
            _ => "❓ Não especificado"
        };
    }

    private string GetPriorityText(Priority priority)
    {
        return priority switch
        {
            Priority.Low => "🟢 Baixa",
            Priority.Medium => "🟡 Média",
            Priority.High => "🟠 Alta",
            Priority.Critical => "🔴 Crítica",
            _ => "❓ Não especificada"
        };
    }

    private string GetOccurrenceTypeText(int occurrenceType)
    {
        return occurrenceType switch
        {
            1 => "🔄 Sempre",
            2 => "⏱️ Às vezes",
            3 => "🎯 Uma vez",
            _ => "❓ Não especificado"
        };
    }
}
