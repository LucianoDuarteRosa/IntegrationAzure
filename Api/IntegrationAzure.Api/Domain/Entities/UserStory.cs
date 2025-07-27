using System.ComponentModel.DataAnnotations;

namespace IntegrationAzure.Api.Domain.Entities;

/// <summary>
/// Entidade que representa uma História de Usuário
/// Baseada no formulário StoryForm.jsx da aplicação React
/// </summary>
public class UserStory : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string DemandNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string AcceptanceCriteria { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Relacionamento com casos de uso
    public virtual ICollection<TestCase> TestCases { get; set; } = new List<TestCase>();

    // Relacionamento com anexos
    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    // Status da história
    public UserStoryStatus Status { get; set; } = UserStoryStatus.New;

    // Prioridade
    public Priority Priority { get; set; } = Priority.Medium;
}

/// <summary>
/// Enumeration para status da história
/// </summary>
public enum UserStoryStatus
{
    New = 1,
    InProgress = 2,
    Testing = 3,
    Completed = 4,
    Rejected = 5
}

/// <summary>
/// Enumeration para prioridade
/// </summary>
public enum Priority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}
