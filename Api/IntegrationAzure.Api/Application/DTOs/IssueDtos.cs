using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Application.DTOs;

public class CreateIssueDto
{
    public string? IssueNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IssueType Type { get; set; }
    public Priority Priority { get; set; }
    public string? Activity { get; set; }
    public string? Environment { get; set; }
    public int? UserStoryId { get; set; }
    public string? Observations { get; set; }
    public List<IssueScenarioDto>? Scenarios { get; set; } = new();
    public List<AttachmentWithContentDto>? Attachments { get; set; } = new();
}

public class UpdateIssueDto
{
    public string? IssueNumber { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public IssueType? Type { get; set; }
    public Priority? Priority { get; set; }
    public IssueStatus? Status { get; set; }
    public string? Activity { get; set; }
    public string? Environment { get; set; }
    public int? UserStoryId { get; set; }
    public string? Observations { get; set; }
    public List<IssueScenarioDto>? Scenarios { get; set; }
}

public class IssueDto
{
    public Guid Id { get; set; }
    public string? IssueNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IssueType Type { get; set; }
    public Priority Priority { get; set; }
    public IssueStatus Status { get; set; }
    public string? Activity { get; set; }
    public string? Environment { get; set; }
    public int? UserStoryId { get; set; }
    public string? CreatedBy { get; set; }
    public List<AttachmentDto>? Attachments { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class IssueSummaryDto
{
    public Guid Id { get; set; }
    public string? IssueNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public IssueType Type { get; set; }
    public Priority Priority { get; set; }
    public IssueStatus Status { get; set; }
    public string? Activity { get; set; }
    public string? Environment { get; set; }
    public int? UserStoryId { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class IssueScenarioDto
{
    public int Id { get; set; }
    public string? Given { get; set; } = string.Empty;
    public string? When { get; set; } = string.Empty;
    public string? Then { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public int IssueId { get; set; }
}

public class IssueAttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}