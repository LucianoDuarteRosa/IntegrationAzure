using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Application.DTOs;

/// <summary>
/// DTO para criação de nova issue
/// </summary>
public class CreateIssueDto
{
    public string IssueNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IssueType Type { get; set; } = IssueType.Bug;
    public Priority Priority { get; set; } = Priority.Medium;
    public string? AssignedTo { get; set; }
    public string? Reporter { get; set; }
    public string? Environment { get; set; }
    public string? StepsToReproduce { get; set; }
    public string? ExpectedResult { get; set; }
    public string? ActualResult { get; set; }
    public Guid? UserStoryId { get; set; }
}

/// <summary>
/// DTO para atualização de issue
/// </summary>
public class UpdateIssueDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public IssueType? Type { get; set; }
    public Priority? Priority { get; set; }
    public IssueStatus? Status { get; set; }
    public string? AssignedTo { get; set; }
    public string? Environment { get; set; }
    public string? StepsToReproduce { get; set; }
    public string? ExpectedResult { get; set; }
    public string? ActualResult { get; set; }
    public string? Resolution { get; set; }
    public Guid? UserStoryId { get; set; }
}

/// <summary>
/// DTO para retorno de issue
/// </summary>
public class IssueDto
{
    public Guid Id { get; set; }
    public string IssueNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IssueType Type { get; set; }
    public Priority Priority { get; set; }
    public IssueStatus Status { get; set; }
    public string? AssignedTo { get; set; }
    public string? Reporter { get; set; }
    public string? Environment { get; set; }
    public string? StepsToReproduce { get; set; }
    public string? ExpectedResult { get; set; }
    public string? ActualResult { get; set; }
    public string? Resolution { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public Guid? UserStoryId { get; set; }
    public UserStorySummaryDto? UserStory { get; set; }
    public List<AttachmentDto> Attachments { get; set; } = new();
}

/// <summary>
/// DTO para resumo de issue (listagem)
/// </summary>
public class IssueSummaryDto
{
    public Guid Id { get; set; }
    public string IssueNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public IssueType Type { get; set; }
    public Priority Priority { get; set; }
    public IssueStatus Status { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UserStoryTitle { get; set; }
    public int AttachmentsCount { get; set; }
}
