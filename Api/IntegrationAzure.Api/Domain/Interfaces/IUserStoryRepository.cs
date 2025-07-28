using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Domain.Interfaces;

/// <summary>
/// Interface específica para repositório de histórias de usuário
/// Extende o repositório genérico com operações específicas
/// </summary>
public interface IUserStoryRepository : IRepository<UserStory>
{
    Task<IEnumerable<UserStory>> GetByDemandNumberAsync(string demandNumber);
    Task<IEnumerable<UserStory>> GetByStatusAsync(UserStoryStatus status);
    Task<IEnumerable<UserStory>> GetByPriorityAsync(Priority priority);
    Task<UserStory?> GetWithAttachmentsAsync(Guid id);
    Task<UserStory?> GetCompleteAsync(Guid id);
}
