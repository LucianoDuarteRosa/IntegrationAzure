using System.ComponentModel.DataAnnotations;

namespace IntegrationAzure.Api.Domain.Entities;

/// <summary>
/// Classe base para todas as entidades do domínio
/// Fornece propriedades comuns como identificação e auditoria
/// </summary>
public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public string? UpdatedBy { get; set; }

    public bool IsActive { get; set; } = true;
}
