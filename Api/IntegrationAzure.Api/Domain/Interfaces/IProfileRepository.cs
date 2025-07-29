using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Domain.Interfaces;

/// <summary>
/// Interface específica para repositório de perfis
/// Estende a interface base com operações específicas para Profile
/// </summary>
public interface IProfileRepository : IRepository<Profile>
{
    Task<Profile?> GetByNameAsync(string name);
    Task<bool> ExistsByNameAsync(string name);
    Task<IEnumerable<Profile>> GetActiveProfilesAsync();
}
