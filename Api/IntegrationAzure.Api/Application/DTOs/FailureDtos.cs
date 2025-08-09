using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Application.DTOs;

/// <summary>
/// DTO para criação de nova falha
/// </summary>
public class CreateFailureDto
{
    public string FailureNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public FailureSeverity Severity { get; set; } = FailureSeverity.Medium;
    // Substitui o antigo OccurrenceType pelo Activity (Azure DevOps)
    public string? Activity { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public string? Environment { get; set; }
    public Guid? UserStoryId { get; set; }
    public List<FailureScenarioDto>? Scenarios { get; set; }
    public string? Observations { get; set; }
    public List<FailureAttachmentDto>? Attachments { get; set; }
}

/// <summary>
/// DTO para representar cenários de falha (Dado que/Quando/Então)
/// </summary>
public class FailureScenarioDto
{
    public string Given { get; set; } = string.Empty;
    public string When { get; set; } = string.Empty;
    public string Then { get; set; } = string.Empty;
}

/// <summary>
/// DTO para representar anexos/evidências de falha
/// </summary>
public class FailureAttachmentDto
{
    public string Name { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Type { get; set; } = string.Empty;
}

/// <summary>
/// DTO para retorno de falha
/// </summary>
public class FailureDto
{
    public Guid Id { get; set; }
    public string FailureNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public FailureSeverity Severity { get; set; }
    public FailureStatus Status { get; set; }
    public string? Activity { get; set; }
    public DateTime OccurredAt { get; set; }
    public string? Environment { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public Guid? UserStoryId { get; set; }
    public UserStorySummaryDto? UserStory { get; set; }
    public List<AttachmentDto> Attachments { get; set; } = new();
}

/// <summary>
/// DTO para resumo de falha (listagem)
/// </summary>
public class FailureSummaryDto
{
    public Guid Id { get; set; }
    public string FailureNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public FailureSeverity Severity { get; set; }
    public FailureStatus Status { get; set; }
    public string? Activity { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UserStoryTitle { get; set; }
    public int AttachmentsCount { get; set; }
}
