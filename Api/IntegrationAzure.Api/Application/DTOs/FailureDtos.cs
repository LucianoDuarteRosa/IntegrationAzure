using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Application.DTOs;

public class CreateFailureDto
{
    public string? FailureNumber { get; set; }
    public string? Title { get; set; }
    public string? Observations { get; set; }
    public string? Activity { get; set; }
    public int? UserStoryId { get; set; }
    public FailureSeverity? Severity { get; set; }
    public DateTime? OccurredAt { get; set; }
    public string? Environment { get; set; }
    public List<FailureScenarioDto>? Scenarios { get; set; } = new();
    public List<AttachmentDto>? Attachments { get; set; } = new();
}

public class UpdateFailureDto
{
    public string? FailureNumber { get; set; }
    public string? Title { get; set; }
    public string? Observations { get; set; }
    public string? Activity { get; set; }
    public int? UserStoryId { get; set; }
    public FailureSeverity? Severity { get; set; }
    public FailureStatus? Status { get; set; }
    public DateTime? OccurredAt { get; set; }
    public string? Environment { get; set; }
    public List<FailureScenarioDto>? Scenarios { get; set; }
}

public class FailureDto
{
    public Guid Id { get; set; }
    public string? FailureNumber { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Observations { get; set; }
    public string? Activity { get; set; }
    public int? UserStoryId { get; set; }
    public FailureSeverity Severity { get; set; }
    public FailureStatus Status { get; set; }
    public DateTime OccurredAt { get; set; }
    public string? Environment { get; set; }
    public List<FailureScenarioDto>? Scenarios { get; set; } = new();
    public List<AttachmentDto>? Attachments { get; set; } = new();
    public string? CreatedBy { get; set; }
    public UserStoryDto? UserStory { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class FailureSummaryDto
{
    public Guid Id { get; set; }
    public string? FailureNumber { get; set; }
    public string? Title { get; set; }
    public string? Activity { get; set; }
    public int? UserStoryId { get; set; }
    public FailureSeverity Severity { get; set; }
    public FailureStatus Status { get; set; }
    public DateTime OccurredAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UserStoryTitle { get; set; }
    public int AttachmentsCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class FailureScenarioDto
{
    public int Id { get; set; }
    public string? Given { get; set; } = string.Empty;
    public string? When { get; set; } = string.Empty;
    public string? Then { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? ExpectedResult { get; set; } = string.Empty;
    public string? ActualResult { get; set; } = string.Empty;
    public string? Steps { get; set; }
    public int FailureId { get; set; }
}