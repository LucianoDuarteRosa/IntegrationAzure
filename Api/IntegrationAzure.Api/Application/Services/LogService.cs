using IntegrationAzure.Api.Application.DTOs;
using IntegrationAzure.Api.Domain.Entities;
using IntegrationAzure.Api.Domain.Interfaces;

namespace IntegrationAzure.Api.Application.Services;

/// <summary>
/// Serviço para operações relacionadas a logs
/// Implementa regras de negócio e orchestração
/// </summary>
public class LogService
{
    private readonly ILogRepository _logRepository;

    public LogService(ILogRepository logRepository)
    {
        _logRepository = logRepository ?? throw new ArgumentNullException(nameof(logRepository));
    }

    /// <summary>
    /// Obtém logs com filtros
    /// </summary>
    public async Task<ApiResponseDto<List<LogSummaryDto>>> GetFilteredAsync(LogFilterDto filter)
    {
        try
        {
            var logs = await _logRepository.GetAllAsync();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(filter.UserId))
                logs = logs.Where(l => l.UserId.Contains(filter.UserId));

            if (!string.IsNullOrEmpty(filter.Entity))
                logs = logs.Where(l => l.Entity.Contains(filter.Entity));

            if (filter.Level.HasValue)
                logs = logs.Where(l => l.Level == filter.Level);

            if (filter.StartDate.HasValue)
                logs = logs.Where(l => l.CreatedAt >= filter.StartDate);

            if (filter.EndDate.HasValue)
                logs = logs.Where(l => l.CreatedAt <= filter.EndDate);

            // Ordenar por data decrescente (mais recentes primeiro)
            logs = logs.OrderByDescending(l => l.CreatedAt);

            // Paginação
            var totalCount = logs.Count();
            var pagedLogs = logs
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(MapToSummaryDto)
                .ToList();

            return new ApiResponseDto<List<LogSummaryDto>>
            {
                Success = true,
                Message = $"{totalCount} logs encontrados",
                Data = pagedLogs
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<List<LogSummaryDto>>
            {
                Success = false,
                Message = "Erro interno do servidor",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    /// <summary>
    /// Método de conveniência para criar logs rapidamente
    /// </summary>
    public async Task LogActionAsync(string action, string entity, string? entityId, string userId, string? details = null, Domain.Entities.LogLevel level = Domain.Entities.LogLevel.Info)
    {
        try
        {
            var log = new Log
            {
                Action = action,
                Entity = entity,
                EntityId = entityId,
                UserId = userId,
                Details = details,
                Level = level,
                CreatedBy = userId
            };

            await _logRepository.AddAsync(log);
            await _logRepository.SaveChangesAsync();
        }
        catch
        {
            // Silenciosamente falha - logs não devem quebrar a aplicação
        }
    }

    /// <summary>
    /// Mapeia entidade para DTO de resumo
    /// </summary>
    private static LogSummaryDto MapToSummaryDto(Log log)
    {
        return new LogSummaryDto
        {
            Id = log.Id,
            Action = log.Action,
            Entity = log.Entity,
            EntityId = log.EntityId,
            UserId = log.UserId,
            Level = log.Level,
            CreatedAt = log.CreatedAt
        };
    }
}
