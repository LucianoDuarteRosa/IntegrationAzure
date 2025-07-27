using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Application.DTOs;

/// <summary>
/// DTO para criação de nova história de usuário
/// Baseado no formulário StoryForm.jsx
/// </summary>
public class CreateUserStoryDto
{
    public string DemandNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string AcceptanceCriteria { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public List<CreateTestCaseDto> TestCases { get; set; } = new();
}

/// <summary>
/// DTO para atualização de história de usuário
/// </summary>
public class UpdateUserStoryDto
{
    public string? Title { get; set; }
    public string? AcceptanceCriteria { get; set; }
    public string? Description { get; set; }
    public UserStoryStatus? Status { get; set; }
    public Priority? Priority { get; set; }
    public List<CreateTestCaseDto>? TestCases { get; set; }
}

/// <summary>
/// DTO para retorno de história de usuário
/// </summary>
public class UserStoryDto
{
    public Guid Id { get; set; }
    public string DemandNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string AcceptanceCriteria { get; set; } = string.Empty;
    public string? Description { get; set; }
    public UserStoryStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public List<TestCaseDto> TestCases { get; set; } = new();
    public List<AttachmentDto> Attachments { get; set; } = new();
}

/// <summary>
/// DTO para resumo de história de usuário (listagem)
/// </summary>
public class UserStorySummaryDto
{
    public Guid Id { get; set; }
    public string DemandNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public UserStoryStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public int TestCasesCount { get; set; }
    public int AttachmentsCount { get; set; }
}
