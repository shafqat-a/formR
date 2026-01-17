namespace FormR.Core.Interfaces;

/// <summary>
/// Repository interface for form-related data operations
/// Implementations will be added in FormR.Data
/// </summary>
public interface IFormRepository
{
    // Template operations
    Task<T?> GetTemplateByIdAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : class;
    Task<IEnumerable<T>> GetTemplatesAsync<T>(Guid tenantId, CancellationToken cancellationToken = default) where T : class;
    Task<T> CreateTemplateAsync<T>(T template, CancellationToken cancellationToken = default) where T : class;
    Task<T> UpdateTemplateAsync<T>(T template, CancellationToken cancellationToken = default) where T : class;
    Task DeleteTemplateAsync(Guid id, CancellationToken cancellationToken = default);

    // Instance operations
    Task<T?> GetInstanceByIdAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : class;
    Task<IEnumerable<T>> GetInstancesAsync<T>(Guid templateId, CancellationToken cancellationToken = default) where T : class;
    Task<T> CreateInstanceAsync<T>(T instance, CancellationToken cancellationToken = default) where T : class;
    Task<T> UpdateInstanceAsync<T>(T instance, CancellationToken cancellationToken = default) where T : class;
}
