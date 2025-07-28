using System.ComponentModel.DataAnnotations;

namespace IntegrationAzure.Api.Domain.Entities;

/// <summary>
/// Entidade que representa um log de ação do sistema
/// Para auditoria e monitoramento de atividades
/// </summary>
public class Log : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Action { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Entity { get; set; } = string.Empty;

    [StringLength(100)]
    public string? EntityId { get; set; }

    [Required]
    [StringLength(100)]
    public string UserId { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Details { get; set; }

    public LogLevel Level { get; set; } = LogLevel.Info;

    [StringLength(50)]
    public string? IpAddress { get; set; }

    [StringLength(500)]
    public string? UserAgent { get; set; }
}

/// <summary>
/// Enumeration para nível do log
/// </summary>
public enum LogLevel
{
    Info = 1,
    Warning = 2,
    Error = 3,
    Success = 4
}
