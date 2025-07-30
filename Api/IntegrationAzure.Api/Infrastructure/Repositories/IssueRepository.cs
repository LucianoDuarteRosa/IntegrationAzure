using Microsoft.EntityFrameworkCore;
using IntegrationAzure.Api.Domain.Entities;
using IntegrationAzure.Api.Domain.Interfaces;
using IntegrationAzure.Api.Infrastructure.Data;

namespace IntegrationAzure.Api.Infrastructure.Repositories;

/// <summary>
/// Implementação específica do repositório para issues
/// Extende o repositório genérico com operações específicas
/// </summary>
public class IssueRepository : Repository<Issue>, IIssueRepository
{
    public IssueRepository(IntegrationAzureDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Issue>> GetByStatusAsync(IssueStatus status)
    {
        return await _dbSet
            .Where(i => i.Status == status)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Issue>> GetByTypeAsync(IssueType type)
    {
        return await _dbSet
            .Where(i => i.Type == type)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Issue>> GetByPriorityAsync(Priority priority)
    {
        return await _dbSet
            .Where(i => i.Priority == priority)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<Issue?> GetWithAttachmentsAsync(Guid id)
    {
        return await _dbSet
            .Include(i => i.Attachments)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Issue>> GetByUserStoryIdAsync(Guid userStoryId)
    {
        return await _dbSet
            .Where(i => i.UserStoryId == userStoryId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Issue>> GetAllAsync()
    {
        return await _dbSet
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }
}
