using FluentValidation;
using FormR.Core.Interfaces;
using FormR.Core.Models;

namespace FormR.API.Services;

public class TemplateService : ITemplateService
{
    private readonly IFormRepository _repository;
    private readonly IValidator<FormTemplate> _templateValidator;
    private readonly IValidator<FormControl> _controlValidator;

    public TemplateService(
        IFormRepository repository,
        IValidator<FormTemplate> templateValidator,
        IValidator<FormControl> controlValidator)
    {
        _repository = repository;
        _templateValidator = templateValidator;
        _controlValidator = controlValidator;
    }

    public async Task<T?> GetByIdAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : class
    {
        if (typeof(T) == typeof(FormTemplate))
        {
            return await _repository.GetTemplateByIdAsync<T>(id, cancellationToken);
        }

        throw new NotSupportedException($"Type {typeof(T).Name} is not supported");
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>(Guid tenantId, CancellationToken cancellationToken = default) where T : class
    {
        if (typeof(T) == typeof(FormTemplate))
        {
            return await _repository.GetTemplatesAsync<T>(tenantId, cancellationToken);
        }

        throw new NotSupportedException($"Type {typeof(T).Name} is not supported");
    }

    public async Task<T> CreateAsync<T>(T template, CancellationToken cancellationToken = default) where T : class
    {
        if (template is FormTemplate formTemplate)
        {
            // Validate template
            var validationResult = await _templateValidator.ValidateAsync(formTemplate, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Validate all controls
            foreach (var control in formTemplate.Controls)
            {
                var controlValidationResult = await _controlValidator.ValidateAsync(control, cancellationToken);
                if (!controlValidationResult.IsValid)
                {
                    throw new ValidationException(controlValidationResult.Errors);
                }
            }

            return await _repository.CreateTemplateAsync(template, cancellationToken);
        }

        throw new NotSupportedException($"Type {typeof(T).Name} is not supported");
    }

    public async Task<T> UpdateAsync<T>(Guid id, T template, CancellationToken cancellationToken = default) where T : class
    {
        if (template is FormTemplate formTemplate)
        {
            // Ensure the ID matches
            formTemplate.Id = id;

            // Validate template
            var validationResult = await _templateValidator.ValidateAsync(formTemplate, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Validate all controls
            foreach (var control in formTemplate.Controls)
            {
                var controlValidationResult = await _controlValidator.ValidateAsync(control, cancellationToken);
                if (!controlValidationResult.IsValid)
                {
                    throw new ValidationException(controlValidationResult.Errors);
                }
            }

            return await _repository.UpdateTemplateAsync(template, cancellationToken);
        }

        throw new NotSupportedException($"Type {typeof(T).Name} is not supported");
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteTemplateAsync(id, cancellationToken);
    }

    public async Task<T> DuplicateTemplateAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : class
    {
        if (typeof(T) == typeof(FormTemplate))
        {
            var original = await _repository.GetTemplateByIdAsync<FormTemplate>(id, cancellationToken);
            if (original == null)
            {
                throw new KeyNotFoundException($"Template with ID {id} not found");
            }

            // Create a duplicate with a new ID
            var duplicate = new FormTemplate
            {
                Name = $"{original.Name} (Copy)",
                Description = original.Description,
                Version = 1,
                TenantId = original.TenantId,
                BaseTemplateId = original.BaseTemplateId,
                Controls = original.Controls.Select(c => new FormControl
                {
                    Type = c.Type,
                    Label = c.Label,
                    Placeholder = c.Placeholder,
                    DefaultValue = c.DefaultValue,
                    IsRequired = c.IsRequired,
                    ValidationRules = c.ValidationRules,
                    Position = c.Position,
                    Properties = c.Properties,
                    ParentControlId = c.ParentControlId,
                    Order = c.Order
                }).ToList()
            };

            var result = await _repository.CreateTemplateAsync(duplicate, cancellationToken);
            return result as T ?? throw new InvalidOperationException("Failed to create template duplicate");
        }

        throw new NotSupportedException($"Type {typeof(T).Name} is not supported");
    }

    public Task<IEnumerable<T>> GetTemplateVersionsAsync<T>(Guid templateId, CancellationToken cancellationToken = default) where T : class
    {
        // Template versioning will be implemented in User Story 4
        throw new NotImplementedException("Template versioning will be implemented in User Story 4");
    }
}
