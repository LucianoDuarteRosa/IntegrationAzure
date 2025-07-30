using Microsoft.EntityFrameworkCore;
using IntegrationAzure.Api.Domain.Entities;
using IntegrationAzure.Api.Domain.Interfaces;
using IntegrationAzure.Api.Infrastructure.Data;

namespace IntegrationAzure.Api.Infrastructure.Repositories;

/// <summary>
/// Implementação específica do repositório para falhas
/// Extende o repositório genérico com operações específicas
/// </summary>
public class FailureRepository : Repository<Failure>, IFailureRepository
{
    public FailureRepository(IntegrationAzureDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Failure>> GetByStatusAsync(FailureStatus status)
    {
        return await _dbSet
            .Where(f => f.Status == status)
            .OrderByDescending(f => f.OccurredAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Failure>> GetBySeverityAsync(FailureSeverity severity)
    {
        return await _dbSet
            .Where(f => f.Severity == severity)
            .OrderByDescending(f => f.OccurredAt)
            .ToListAsync();
    }

    public async Task<Failure?> GetWithAttachmentsAsync(Guid id)
    {
        return await _dbSet
            .Include(f => f.Attachments)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<IEnumerable<Failure>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(f => f.OccurredAt >= startDate && f.OccurredAt <= endDate)
            .OrderByDescending(f => f.OccurredAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Failure>> GetCriticalFailuresAsync()
    {
        return await _dbSet
            .Where(f => f.Severity == FailureSeverity.Critical)
            .OrderByDescending(f => f.OccurredAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Failure>> GetUnresolvedFailuresAsync()
    {
        var unresolvedStatuses = new[]
        {
            FailureStatus.Reported,
            FailureStatus.Investigating,
            FailureStatus.InProgress,
            FailureStatus.Monitoring
        };

        return await _dbSet
            .Where(f => unresolvedStatuses.Contains(f.Status))
            .OrderByDescending(f => f.OccurredAt)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Failure>> GetAllAsync()
    {
        return await _dbSet
            .Include(f => f.UserStory)
            .OrderByDescending(f => f.OccurredAt)
            .ToListAsync();
    }
}
