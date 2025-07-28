using Microsoft.EntityFrameworkCore;
using IntegrationAzure.Api.Domain.Entities;
using IntegrationAzure.Api.Domain.Interfaces;
using IntegrationAzure.Api.Infrastructure.Data;

namespace IntegrationAzure.Api.Infrastructure.Repositories;

/// <summary>
/// Implementação específica do repositório para histórias de usuário
/// Extende o repositório genérico com operações específicas
/// </summary>
public class UserStoryRepository : Repository<UserStory>, IUserStoryRepository
{
    public UserStoryRepository(IntegrationAzureDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserStory>> GetByDemandNumberAsync(string demandNumber)
    {
        if (string.IsNullOrWhiteSpace(demandNumber))
            throw new ArgumentException("Demand number cannot be null or empty", nameof(demandNumber));

        return await _dbSet
            .Where(us => us.DemandNumber == demandNumber)
            .OrderByDescending(us => us.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserStory>> GetByStatusAsync(UserStoryStatus status)
    {
        return await _dbSet
            .Where(us => us.Status == status)
            .OrderByDescending(us => us.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserStory>> GetByPriorityAsync(Priority priority)
    {
        return await _dbSet
            .Where(us => us.Priority == priority)
            .OrderByDescending(us => us.CreatedAt)
            .ToListAsync();
    }

    public async Task<UserStory?> GetWithAttachmentsAsync(Guid id)
    {
        return await _dbSet
            .Include(us => us.Attachments)
            .FirstOrDefaultAsync(us => us.Id == id);
    }

    public async Task<UserStory?> GetCompleteAsync(Guid id)
    {
        return await _dbSet
            .Include(us => us.Attachments)
            .FirstOrDefaultAsync(us => us.Id == id);
    }

    public override async Task<IEnumerable<UserStory>> GetAllAsync()
    {
        return await _dbSet
            .OrderByDescending(us => us.CreatedAt)
            .ToListAsync();
    }
}
