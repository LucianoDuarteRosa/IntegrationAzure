using IntegrationAzure.Api.Domain.Entities;
using IntegrationAzure.Api.Domain.Interfaces;
using IntegrationAzure.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IntegrationAzure.Api.Infrastructure.Repositories
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly IntegrationAzureDbContext _context;

        public ConfigurationRepository(IntegrationAzureDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Configuration>> GetAllAsync()
        {
            return await _context.Configurations
                .OrderBy(c => c.Category)
                .ThenBy(c => c.Key)
                .ToListAsync();
        }

        public async Task<Configuration?> GetByIdAsync(int id)
        {
            return await _context.Configurations
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Configuration?> GetByKeyAsync(string key)
        {
            return await _context.Configurations
                .FirstOrDefaultAsync(c => c.Key == key && c.IsActive);
        }

        public async Task<IEnumerable<Configuration>> GetByCategoryAsync(string category)
        {
            return await _context.Configurations
                .Where(c => c.Category == category && c.IsActive)
                .OrderBy(c => c.Key)
                .ToListAsync();
        }

        public async Task<IEnumerable<Configuration>> GetByKeysAsync(string[] keys)
        {
            return await _context.Configurations
                .Where(c => keys.Contains(c.Key) && c.IsActive)
                .ToListAsync();
        }

        public async Task<Configuration> CreateAsync(Configuration configuration)
        {
            configuration.CreatedAt = DateTime.UtcNow;
            configuration.UpdatedAt = DateTime.UtcNow;

            _context.Configurations.Add(configuration);
            await _context.SaveChangesAsync();
            return configuration;
        }

        public async Task<Configuration> UpdateAsync(Configuration configuration)
        {
            configuration.UpdatedAt = DateTime.UtcNow;

            _context.Configurations.Update(configuration);
            await _context.SaveChangesAsync();
            return configuration;
        }

        public async Task DeleteAsync(int id)
        {
            var configuration = await GetByIdAsync(id);
            if (configuration != null)
            {
                _context.Configurations.Remove(configuration);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _context.Configurations
                .AnyAsync(c => c.Key == key);
        }
    }
}
