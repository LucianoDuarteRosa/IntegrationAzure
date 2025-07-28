using Microsoft.EntityFrameworkCore;
using IntegrationAzure.Api.Domain.Entities;
using IntegrationAzure.Api.Domain.Interfaces;
using IntegrationAzure.Api.Infrastructure.Data;

namespace IntegrationAzure.Api.Infrastructure.Repositories;

/// <summary>
/// Implementação específica do repositório para logs
/// Extende o repositório genérico com operações específicas
/// </summary>
public class LogRepository : Repository<Log>, ILogRepository
{
    public LogRepository(IntegrationAzureDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Log>> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Log>> GetByEntityAsync(string entity)
    {
        return await _dbSet
            .Where(l => l.Entity == entity)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Log>> GetByLevelAsync(Domain.Entities.LogLevel level)
    {
        return await _dbSet
            .Where(l => l.Level == level)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Log>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(l => l.CreatedAt >= startDate && l.CreatedAt <= endDate)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Log>> GetRecentLogsAsync(int count = 100)
    {
        return await _dbSet
            .OrderByDescending(l => l.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Log>> GetAllAsync()
    {
        return await _dbSet
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }
}
