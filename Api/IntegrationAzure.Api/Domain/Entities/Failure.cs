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

    public int OccurrenceType { get; set; } = 5;

    [Required]
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    public string? Environment { get; set; }

    // Relacionamento com anexos
    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    // Para associar a uma história (opcional) - apenas ID, sem FK constraint
    public Guid? UserStoryId { get; set; }
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

/// <summary>
/// Enumeration para tipo de ocorrência da falha
/// </summary>
public enum FailureOccurrenceType
{
    ApoioOperacional = 1,
    Desempenho = 2,
    DuvidaOuErroDeProcedimento = 3,
    ErroDeMigracaoDeDados = 4,
    ErroDeSistema = 5,
    ErroEmAmbiente = 6,
    ProblemaDeBancoDeDados = 7,
    ProblemaDeInfraestrutura = 8,
    ProblemaDeParametrizacoes = 9
}
