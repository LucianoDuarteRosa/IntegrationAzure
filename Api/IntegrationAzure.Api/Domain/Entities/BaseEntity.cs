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

/// <summary>
/// Classe base simplificada para entidades que não necessitam de auditoria completa
/// Usada para UserStory que não terá atualizações
/// </summary>
public abstract class SimpleBaseEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string CreatedBy { get; set; } = string.Empty;
}
