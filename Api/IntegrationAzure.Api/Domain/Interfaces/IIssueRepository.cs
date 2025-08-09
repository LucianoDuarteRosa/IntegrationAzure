using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Domain.Interfaces;

/// <summary>
/// Interface específica para repositório de issues
/// Extende o repositório genérico com operações específicas
/// </summary>
public interface IIssueRepository : IRepository<Issue>
{
    Task<IEnumerable<Issue>> GetByStatusAsync(IssueStatus status);
    Task<IEnumerable<Issue>> GetByTypeAsync(IssueType type);
    Task<IEnumerable<Issue>> GetByPriorityAsync(Priority priority);
    Task<Issue?> GetWithAttachmentsAsync(Guid id);
    Task<IEnumerable<Issue>> GetByUserStoryIdAsync(int userStoryId);
}
