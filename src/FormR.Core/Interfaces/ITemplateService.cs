namespace FormR.Core.Interfaces;

/// <summary>
/// Service interface for template business logic
/// Implementations will be added in FormR.API/Services
/// </summary>
public interface ITemplateService
{
    // Template CRUD operations
    Task<T?> GetByIdAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : class;
    Task<IEnumerable<T>> GetAllAsync<T>(Guid tenantId, CancellationToken cancellationToken = default) where T : class;
    Task<T> CreateAsync<T>(T template, CancellationToken cancellationToken = default) where T : class;
    Task<T> UpdateAsync<T>(Guid id, T template, CancellationToken cancellationToken = default) where T : class;
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    // Template versioning (for User Story 4)
    Task<T> DuplicateTemplateAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : class;
    Task<IEnumerable<T>> GetTemplateVersionsAsync<T>(Guid templateId, CancellationToken cancellationToken = default) where T : class;
}
