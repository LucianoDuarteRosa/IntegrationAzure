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
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public string? ReportedBy { get; set; }
    public string? AssignedTo { get; set; }
    public string? Environment { get; set; }
    public string? SystemsAffected { get; set; }
    public string? ImpactDescription { get; set; }
    public string? StepsToReproduce { get; set; }
    public string? WorkaroundSolution { get; set; }
    public Guid? IssueId { get; set; }
    public Guid? UserStoryId { get; set; }
    public decimal? EstimatedImpactCost { get; set; }
}

/// <summary>
/// DTO para atualização de falha
/// </summary>
public class UpdateFailureDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public FailureSeverity? Severity { get; set; }
    public FailureStatus? Status { get; set; }
    public string? AssignedTo { get; set; }
    public string? Environment { get; set; }
    public string? SystemsAffected { get; set; }
    public string? ImpactDescription { get; set; }
    public string? StepsToReproduce { get; set; }
    public string? WorkaroundSolution { get; set; }
    public string? RootCauseAnalysis { get; set; }
    public string? PermanentSolution { get; set; }
    public Guid? IssueId { get; set; }
    public Guid? UserStoryId { get; set; }
    public TimeSpan? DowntimeDuration { get; set; }
    public decimal? EstimatedImpactCost { get; set; }
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
    public DateTime OccurredAt { get; set; }
    public string? ReportedBy { get; set; }
    public string? AssignedTo { get; set; }
    public string? Environment { get; set; }
    public string? SystemsAffected { get; set; }
    public string? ImpactDescription { get; set; }
    public string? StepsToReproduce { get; set; }
    public string? WorkaroundSolution { get; set; }
    public string? RootCauseAnalysis { get; set; }
    public string? PermanentSolution { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public TimeSpan? DowntimeDuration { get; set; }
    public decimal? EstimatedImpactCost { get; set; }
    public Guid? IssueId { get; set; }
    public IssueSummaryDto? Issue { get; set; }
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
    public DateTime OccurredAt { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public TimeSpan? DowntimeDuration { get; set; }
    public string? IssueTitle { get; set; }
    public string? UserStoryTitle { get; set; }
    public int AttachmentsCount { get; set; }
}
