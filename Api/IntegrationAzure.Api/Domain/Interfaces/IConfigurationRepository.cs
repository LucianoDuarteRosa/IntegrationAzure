using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Domain.Interfaces
{
    public interface IConfigurationRepository
    {
        Task<IEnumerable<Configuration>> GetAllAsync();
        Task<Configuration?> GetByIdAsync(int id);
        Task<Configuration?> GetByKeyAsync(string key);
        Task<IEnumerable<Configuration>> GetByCategoryAsync(string category);
        Task<Configuration> CreateAsync(Configuration configuration);
        Task<Configuration> UpdateAsync(Configuration configuration);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(string key);
    }
}
