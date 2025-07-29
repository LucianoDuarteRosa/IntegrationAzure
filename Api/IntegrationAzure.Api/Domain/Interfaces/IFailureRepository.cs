using IntegrationAzure.Api.Domain.Entities;

namespace IntegrationAzure.Api.Domain.Interfaces;

/// <summary>
/// Interface específica para repositório de falhas
/// Extende o repositório genérico com operações específicas
/// </summary>
public interface IFailureRepository : IRepository<Failure>
{
    Task<IEnumerable<Failure>> GetByStatusAsync(FailureStatus status);
    Task<IEnumerable<Failure>> GetBySeverityAsync(FailureSeverity severity);
    Task<Failure?> GetWithAttachmentsAsync(Guid id);
    Task<IEnumerable<Failure>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Failure>> GetCriticalFailuresAsync();
    Task<IEnumerable<Failure>> GetUnresolvedFailuresAsync();
}
