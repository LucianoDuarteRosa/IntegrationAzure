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
    public int OccurrenceType { get; set; } = 5;
    public string? Environment { get; set; }
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
    public int? OccurrenceType { get; set; }
    public string? Environment { get; set; }
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
    public int OccurrenceType { get; set; }
    public string? Environment { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
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
    public int OccurrenceType { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UserStoryTitle { get; set; }
    public int AttachmentsCount { get; set; }
}
