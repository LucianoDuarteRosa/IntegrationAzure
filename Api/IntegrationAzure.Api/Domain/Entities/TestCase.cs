using System.ComponentModel.DataAnnotations;

namespace IntegrationAzure.Api.Domain.Entities;

/// <summary>
/// Entidade que representa um caso de teste/uso
/// Baseada nos casos dinâmicos do formulário de histórias
/// </summary>
public class TestCase : BaseEntity
{
    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public Guid UserStoryId { get; set; }

    public virtual UserStory UserStory { get; set; } = null!;

    public int OrderIndex { get; set; }

    public TestCaseStatus Status { get; set; } = TestCaseStatus.Pending;

    public string? Result { get; set; }

    public string? Notes { get; set; }
}

/// <summary>
/// Enumeration para status do caso de teste
/// </summary>
public enum TestCaseStatus
{
    Pending = 1,
    Passed = 2,
    Failed = 3,
    Blocked = 4
}
