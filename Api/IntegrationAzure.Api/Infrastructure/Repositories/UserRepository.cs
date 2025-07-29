using Microsoft.EntityFrameworkCore;
using IntegrationAzure.Api.Domain.Entities;
using IntegrationAzure.Api.Domain.Interfaces;
using IntegrationAzure.Api.Infrastructure.Data;

namespace IntegrationAzure.Api.Infrastructure.Repositories;

/// <summary>
/// Implementação específica do repositório para User
/// Contém operações específicas para manipulação de usuários
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(IntegrationAzureDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
    }

    public async Task<User?> GetByNicknameAsync(string nickname)
    {
        return await _dbSet
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Nickname.ToLower() == nickname.ToLower() && u.IsActive);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbSet
            .AnyAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
    }

    public async Task<bool> ExistsByNicknameAsync(string nickname)
    {
        return await _dbSet
            .AnyAsync(u => u.Nickname.ToLower() == nickname.ToLower() && u.IsActive);
    }

    public async Task<IEnumerable<User>> GetByProfileIdAsync(Guid profileId)
    {
        return await _dbSet
            .Include(u => u.Profile)
            .Where(u => u.ProfileId == profileId && u.IsActive)
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    public async Task<User?> GetWithProfileAsync(Guid id)
    {
        return await _dbSet
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
    }

    public async Task<IEnumerable<User>> GetAllWithProfilesAsync()
    {
        return await _dbSet
            .Include(u => u.Profile)
            .Where(u => u.IsActive)
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    public override async Task<IEnumerable<User>> GetAllAsync()
    {
        return await GetAllWithProfilesAsync();
    }

    public override async Task<User?> GetByIdAsync(Guid id)
    {
        return await GetWithProfileAsync(id);
    }
}
