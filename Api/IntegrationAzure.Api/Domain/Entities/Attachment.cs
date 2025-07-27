using System.ComponentModel.DataAnnotations;

namespace IntegrationAzure.Api.Domain.Entities;

/// <summary>
/// Entidade que representa um anexo/arquivo
/// Suporta anexos para histórias, issues e falhas
/// </summary>
public class Attachment : BaseEntity
{
    [Required]
    [StringLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ContentType { get; set; } = string.Empty;

    public long Size { get; set; }

    [Required]
    public string FilePath { get; set; } = string.Empty;

    // Para identificar a qual entidade o anexo pertence
    public Guid? UserStoryId { get; set; }
    public virtual UserStory? UserStory { get; set; }

    public Guid? IssueId { get; set; }
    public virtual Issue? Issue { get; set; }

    public Guid? FailureId { get; set; }
    public virtual Failure? Failure { get; set; }

    public string? Description { get; set; }
}
