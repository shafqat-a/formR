using FormR.Core.Interfaces;
using FormR.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FormR.Data.Repositories;

public class FormRepository : IFormRepository
{
    private readonly FormBuilderContext _context;

    public FormRepository(FormBuilderContext context)
    {
        _context = context;
    }

    #region Template Operations

    public async Task<T?> GetTemplateByIdAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : class
    {
        if (typeof(T) == typeof(FormTemplate))
        {
            var template = await _context.FormTemplates
                .Include(t => t.Controls)
                .Include(t => t.BaseTemplate)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
            return template as T;
        }

        throw new NotSupportedException($"Type {typeof(T).Name} is not supported for template operations");
    }

    public async Task<IEnumerable<T>> GetTemplatesAsync<T>(Guid tenantId, CancellationToken cancellationToken = default) where T : class
    {
        if (typeof(T) == typeof(FormTemplate))
        {
            var templates = await _context.FormTemplates
                .Include(t => t.Controls)
                .Where(t => t.TenantId == tenantId && !t.IsDeleted)
                .OrderByDescending(t => t.ModifiedDate)
                .ToListAsync(cancellationToken);
            return templates.Cast<T>().ToList();
        }

        throw new NotSupportedException($"Type {typeof(T).Name} is not supported for template operations");
    }

    public async Task<T> CreateTemplateAsync<T>(T template, CancellationToken cancellationToken = default) where T : class
    {
        if (template is FormTemplate formTemplate)
        {
            formTemplate.Id = Guid.NewGuid();
            formTemplate.CreatedDate = DateTime.UtcNow;
            formTemplate.ModifiedDate = DateTime.UtcNow;
            formTemplate.IsDeleted = false;

            // Assign IDs to controls
            foreach (var control in formTemplate.Controls)
            {
                control.Id = Guid.NewGuid();
                control.TemplateId = formTemplate.Id;
            }

            await _context.FormTemplates.AddAsync(formTemplate, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return (await GetTemplateByIdAsync<T>(formTemplate.Id, cancellationToken))!;
        }

        throw new NotSupportedException($"Type {typeof(T).Name} is not supported for template operations");
    }

    public async Task<T> UpdateTemplateAsync<T>(T template, CancellationToken cancellationToken = default) where T : class
    {
        if (template is FormTemplate formTemplate)
        {
            var existing = await _context.FormTemplates
                .Include(t => t.Controls)
                .FirstOrDefaultAsync(t => t.Id == formTemplate.Id && !t.IsDeleted, cancellationToken);

            if (existing == null)
            {
                throw new KeyNotFoundException($"Template with ID {formTemplate.Id} not found");
            }

            // Update template properties
            existing.Name = formTemplate.Name;
            existing.Description = formTemplate.Description;
            existing.Version = formTemplate.Version;
            existing.ModifiedDate = DateTime.UtcNow;

            // Remove controls that are no longer present
            var incomingControlIds = formTemplate.Controls.Select(c => c.Id).ToHashSet();
            var controlsToRemove = existing.Controls.Where(c => !incomingControlIds.Contains(c.Id)).ToList();
            foreach (var control in controlsToRemove)
            {
                _context.FormControls.Remove(control);
            }

            // Update or add controls
            foreach (var control in formTemplate.Controls)
            {
                var existingControl = existing.Controls.FirstOrDefault(c => c.Id == control.Id);
                if (existingControl != null)
                {
                    // Update existing control
                    existingControl.Type = control.Type;
                    existingControl.Label = control.Label;
                    existingControl.Placeholder = control.Placeholder;
                    existingControl.DefaultValue = control.DefaultValue;
                    existingControl.IsRequired = control.IsRequired;
                    existingControl.ValidationRules = control.ValidationRules;
                    existingControl.Position = control.Position;
                    existingControl.Properties = control.Properties;
                    existingControl.ParentControlId = control.ParentControlId;
                    existingControl.Order = control.Order;
                }
                else
                {
                    // Add new control
                    control.Id = control.Id == Guid.Empty ? Guid.NewGuid() : control.Id;
                    control.TemplateId = formTemplate.Id;
                    existing.Controls.Add(control);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return (await GetTemplateByIdAsync<T>(formTemplate.Id, cancellationToken))!;
        }

        throw new NotSupportedException($"Type {typeof(T).Name} is not supported for template operations");
    }

    public async Task DeleteTemplateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var template = await _context.FormTemplates
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);

        if (template == null)
        {
            throw new KeyNotFoundException($"Template with ID {id} not found");
        }

        // Soft delete
        template.IsDeleted = true;
        template.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Instance Operations

    public Task<T?> GetInstanceByIdAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : class
    {
        // Instance operations will be implemented in User Story 2
        throw new NotImplementedException("Instance operations will be implemented in User Story 2");
    }

    public Task<IEnumerable<T>> GetInstancesAsync<T>(Guid templateId, CancellationToken cancellationToken = default) where T : class
    {
        // Instance operations will be implemented in User Story 2
        throw new NotImplementedException("Instance operations will be implemented in User Story 2");
    }

    public Task<T> CreateInstanceAsync<T>(T instance, CancellationToken cancellationToken = default) where T : class
    {
        // Instance operations will be implemented in User Story 2
        throw new NotImplementedException("Instance operations will be implemented in User Story 2");
    }

    public Task<T> UpdateInstanceAsync<T>(T instance, CancellationToken cancellationToken = default) where T : class
    {
        // Instance operations will be implemented in User Story 2
        throw new NotImplementedException("Instance operations will be implemented in User Story 2");
    }

    #endregion
}
