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
    /// Cria um novo log
    /// </summary>
    public async Task<ApiResponseDto<LogDto>> CreateAsync(CreateLogDto dto, string userId, string? ipAddress = null, string? userAgent = null)
    {
        try
        {
            var log = new Log
            {
                Action = dto.Action,
                Entity = dto.Entity,
                EntityId = dto.EntityId,
                UserId = userId,
                Details = dto.Details,
                Level = dto.Level,
                IpAddress = ipAddress,
                UserAgent = userAgent?.Length > 500 ? userAgent.Substring(0, 500) : userAgent,
                CreatedBy = userId
            };

            await _logRepository.AddAsync(log);
            await _logRepository.SaveChangesAsync();

            return new ApiResponseDto<LogDto>
            {
                Success = true,
                Message = "Log criado com sucesso",
                Data = MapToDto(log)
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<LogDto>
            {
                Success = false,
                Message = "Erro interno do servidor",
                Errors = new List<string> { ex.Message }
            };
        }
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
    /// Obtém logs recentes
    /// </summary>
    public async Task<ApiResponseDto<List<LogSummaryDto>>> GetRecentAsync(int count = 100)
    {
        try
        {
            var logs = await _logRepository.GetRecentLogsAsync(count);
            var logDtos = logs.Select(MapToSummaryDto).ToList();

            return new ApiResponseDto<List<LogSummaryDto>>
            {
                Success = true,
                Message = $"{logDtos.Count} logs encontrados",
                Data = logDtos
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
            var dto = new CreateLogDto
            {
                Action = action,
                Entity = entity,
                EntityId = entityId,
                Details = details,
                Level = level
            };

            await CreateAsync(dto, userId);
        }
        catch
        {
            // Silenciosamente falha - logs não devem quebrar a aplicação
        }
    }

    /// <summary>
    /// Mapeia entidade para DTO
    /// </summary>
    private static LogDto MapToDto(Log log)
    {
        return new LogDto
        {
            Id = log.Id,
            Action = log.Action,
            Entity = log.Entity,
            EntityId = log.EntityId,
            UserId = log.UserId,
            Details = log.Details,
            Level = log.Level,
            IpAddress = log.IpAddress,
            UserAgent = log.UserAgent,
            CreatedAt = log.CreatedAt
        };
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
