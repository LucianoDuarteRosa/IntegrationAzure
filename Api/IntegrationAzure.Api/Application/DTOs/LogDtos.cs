using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Application.DTOs;

/// <summary>
/// DTO para criação de novo log
/// </summary>
public class CreateLogDto
{
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? Details { get; set; }
    public Domain.Entities.LogLevel Level { get; set; } = Domain.Entities.LogLevel.Info;
}

/// <summary>
/// DTO para retorno de log
/// </summary>
public class LogDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? Details { get; set; }
    public Domain.Entities.LogLevel Level { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO para resumo de log (listagem)
/// </summary>
public class LogSummaryDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Domain.Entities.LogLevel Level { get; set; }
    public DateTime CreatedAt { get; set; }
    public string LevelName => Level.ToString();
}

/// <summary>
/// DTO para filtros de log
/// </summary>
public class LogFilterDto
{
    public string? UserId { get; set; }
    public string? Entity { get; set; }
    public Domain.Entities.LogLevel? Level { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageSize { get; set; } = 50;
    public int PageNumber { get; set; } = 1;
}
