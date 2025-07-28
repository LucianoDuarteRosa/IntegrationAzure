using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Domain.Interfaces;

/// <summary>
/// Interface específica para repositório de logs
/// Extende o repositório genérico com operações específicas
/// </summary>
public interface ILogRepository : IRepository<Log>
{
    Task<IEnumerable<Log>> GetByUserIdAsync(string userId);
    Task<IEnumerable<Log>> GetByEntityAsync(string entity);
    Task<IEnumerable<Log>> GetByLevelAsync(Domain.Entities.LogLevel level);
    Task<IEnumerable<Log>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Log>> GetRecentLogsAsync(int count = 100);
}
