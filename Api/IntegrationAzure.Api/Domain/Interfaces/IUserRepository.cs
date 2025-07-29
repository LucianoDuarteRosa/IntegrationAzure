using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Domain.Interfaces;

/// <summary>
/// Interface específica para repositório de usuários
/// Estende a interface base com operações específicas para User
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByNicknameAsync(string nickname);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByNicknameAsync(string nickname);
    Task<IEnumerable<User>> GetByProfileIdAsync(Guid profileId);
    Task<User?> GetWithProfileAsync(Guid id);
    Task<IEnumerable<User>> GetAllWithProfilesAsync();
}
