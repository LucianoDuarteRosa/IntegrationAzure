using Microsoft.EntityFrameworkCore;
using IntegrationAzure.Api.Domain.Entities;
using IntegrationAzure.Api.Domain.Interfaces;
using IntegrationAzure.Api.Infrastructure.Data;

namespace IntegrationAzure.Api.Infrastructure.Repositories;

/// <summary>
/// Implementação específica do repositório para Profile
/// Contém operações específicas para manipulação de perfis
/// </summary>
public class ProfileRepository : Repository<Profile>, IProfileRepository
{
    public ProfileRepository(IntegrationAzureDbContext context) : base(context)
    {
    }

    public async Task<Profile?> GetByNameAsync(string name)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower() && p.IsActive);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _dbSet
            .AnyAsync(p => p.Name.ToLower() == name.ToLower() && p.IsActive);
    }

    public async Task<IEnumerable<Profile>> GetActiveProfilesAsync()
    {
        return await _dbSet
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Profile>> GetAllAsync()
    {
        return await _dbSet
            .Where(p => p.IsActive)
            .Include(p => p.Users.Where(u => u.IsActive))
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public override async Task<Profile?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.Users.Where(u => u.IsActive))
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
    }
}
