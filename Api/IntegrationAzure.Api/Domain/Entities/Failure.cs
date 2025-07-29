using System.ComponentModel.DataAnnotations;

namespace IntegrationAzure.Api.Domain.Entities;

/// <summary>
/// Entidade que representa uma Falha/Incidente
/// Para rastreamento de falhas em produção e incidentes críticos
/// </summary>
public class Failure : SimpleBaseEntity
{
    [Required]
    [StringLength(50)]
    public string FailureNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public FailureSeverity Severity { get; set; } = FailureSeverity.Medium;

    public FailureStatus Status { get; set; } = FailureStatus.Reported;

    [Required]
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? ReportedBy { get; set; }

    public string? Environment { get; set; }

    // Relacionamento com anexos
    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    // Para associar a uma issue ou história (opcional)
    public Guid? IssueId { get; set; }
    public virtual Issue? Issue { get; set; }

    public Guid? UserStoryId { get; set; }
    public virtual UserStory? UserStory { get; set; }
}

/// <summary>
/// Enumeration para severidade da falha
/// </summary>
public enum FailureSeverity
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

/// <summary>
/// Enumeration para status da falha
/// </summary>
public enum FailureStatus
{
    Reported = 1,
    Investigating = 2,
    InProgress = 3,
    Resolved = 4,
    Closed = 5,
    Monitoring = 6
}
