using System.ComponentModel.DataAnnotations;

namespace IntegrationAzure.Api.Domain.Entities;

/// <summary>
/// Entidade que representa uma Issue/Problema
/// Para rastreamento de bugs, melhorias e tarefas
/// </summary>
public class Issue : SimpleBaseEntity
{
    [Required]
    [StringLength(50)]
    public string IssueNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public IssueType Type { get; set; } = IssueType.Bug;

    public Priority Priority { get; set; } = Priority.Medium;

    public IssueStatus Status { get; set; } = IssueStatus.Open;

    public int OccurrenceType { get; set; } = 5;

    public string? Environment { get; set; }

    // Relacionamento com anexos
    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    // Para associar a uma história específica (opcional) - apenas ID, sem FK constraint
    public Guid? UserStoryId { get; set; }
}

/// <summary>
/// Enumeration para tipo de issue
/// </summary>
public enum IssueType
{
    Bug = 1,
    Feature = 2,
    Improvement = 3,
    Task = 4
}

/// <summary>
/// Enumeration para status da issue
/// </summary>
public enum IssueStatus
{
    Open = 1,
    InProgress = 2,
    Testing = 3,
    Resolved = 4,
    Closed = 5,
    Reopened = 6
}
