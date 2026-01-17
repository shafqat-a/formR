using FormR.API.DTOs;
using FormR.Core.Interfaces;
using FormR.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormR.API.Controllers;

[ApiController]
[Route("api/v1/templates")]
// [Authorize]  // Temporarily disabled for testing - TODO: Re-enable for production
public class TemplatesController : ControllerBase
{
    private readonly ITemplateService _templateService;
    private readonly ILogger<TemplatesController> _logger;

    public TemplatesController(
        ITemplateService templateService,
        ILogger<TemplatesController> logger)
    {
        _templateService = templateService;
        _logger = logger;
    }

    /// <summary>
    /// Get all templates for the current tenant
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TemplateListDto>>> List(CancellationToken cancellationToken)
    {
        try
        {
            // Get tenantId from HttpContext (set by tenant resolution middleware)
            // For testing without auth, use a default tenant ID
            var tenantId = HttpContext.Items["TenantId"] as Guid? ?? Guid.Parse("00000000-0000-0000-0000-000000000001");

            var templates = await _templateService.GetAllAsync<FormTemplate>(tenantId, cancellationToken);

            var result = templates.Select(t => new TemplateListDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Version = t.Version,
                CreatedDate = t.CreatedDate,
                ModifiedDate = t.ModifiedDate,
                ControlCount = t.Controls.Count
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving templates");
            return StatusCode(500, "An error occurred while retrieving templates");
        }
    }

    /// <summary>
    /// Get a template by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TemplateDetailDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var template = await _templateService.GetByIdAsync<FormTemplate>(id, cancellationToken);

            if (template == null)
            {
                return NotFound($"Template with ID {id} not found");
            }

            // Verify tenant access
            var tenantId = HttpContext.Items["TenantId"] as Guid? ?? Guid.Parse("00000000-0000-0000-0000-000000000001");
            if (template.TenantId != tenantId)
            {
                return Forbid();
            }

            var result = new TemplateDetailDto
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                Version = template.Version,
                BaseTemplateId = template.BaseTemplateId,
                CreatedDate = template.CreatedDate,
                ModifiedDate = template.ModifiedDate,
                Controls = template.Controls.Select(c => new ControlDto
                {
                    Id = c.Id,
                    Type = c.Type.ToString(),
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

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving template {TemplateId}", id);
            return StatusCode(500, "An error occurred while retrieving the template");
        }
    }

    /// <summary>
    /// Create a new template
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TemplateDetailDto>> Create(
        [FromBody] CreateTemplateDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = HttpContext.Items["TenantId"] as Guid? ?? Guid.Parse("00000000-0000-0000-0000-000000000001");

            var template = new FormTemplate
            {
                Name = dto.Name,
                Description = dto.Description,
                TenantId = tenantId,
                Controls = dto.Controls.Select(c => new FormControl
                {
                    Type = Enum.Parse<ControlType>(c.Type),
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

            var created = await _templateService.CreateAsync<FormTemplate>(template, cancellationToken);

            var result = new TemplateDetailDto
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                Version = created.Version,
                BaseTemplateId = created.BaseTemplateId,
                CreatedDate = created.CreatedDate,
                ModifiedDate = created.ModifiedDate,
                Controls = created.Controls.Select(c => new ControlDto
                {
                    Id = c.Id,
                    Type = c.Type.ToString(),
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

            return CreatedAtAction(nameof(Get), new { id = created.Id }, result);
        }
        catch (FluentValidation.ValidationException vex)
        {
            return BadRequest(new
            {
                errors = vex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating template");
            return StatusCode(500, "An error occurred while creating the template");
        }
    }

    /// <summary>
    /// Update an existing template
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<TemplateDetailDto>> Update(
        Guid id,
        [FromBody] UpdateTemplateDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = HttpContext.Items["TenantId"] as Guid? ?? Guid.Parse("00000000-0000-0000-0000-000000000001");

            // First check if the template exists and belongs to the tenant
            var existing = await _templateService.GetByIdAsync<FormTemplate>(id, cancellationToken);
            if (existing == null)
            {
                return NotFound($"Template with ID {id} not found");
            }

            if (existing.TenantId != tenantId)
            {
                return Forbid();
            }

            var template = new FormTemplate
            {
                Id = id,
                Name = dto.Name,
                Description = dto.Description,
                Version = dto.Version,
                TenantId = tenantId,
                Controls = dto.Controls.Select(c => new FormControl
                {
                    Id = c.Id,
                    Type = Enum.Parse<ControlType>(c.Type),
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

            var updated = await _templateService.UpdateAsync<FormTemplate>(id, template, cancellationToken);

            var result = new TemplateDetailDto
            {
                Id = updated.Id,
                Name = updated.Name,
                Description = updated.Description,
                Version = updated.Version,
                BaseTemplateId = updated.BaseTemplateId,
                CreatedDate = updated.CreatedDate,
                ModifiedDate = updated.ModifiedDate,
                Controls = updated.Controls.Select(c => new ControlDto
                {
                    Id = c.Id,
                    Type = c.Type.ToString(),
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

            return Ok(result);
        }
        catch (FluentValidation.ValidationException vex)
        {
            return BadRequest(new
            {
                errors = vex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Template with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating template {TemplateId}", id);
            return StatusCode(500, "An error occurred while updating the template");
        }
    }

    /// <summary>
    /// Delete a template
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = HttpContext.Items["TenantId"] as Guid? ?? Guid.Parse("00000000-0000-0000-0000-000000000001");

            // First check if the template exists and belongs to the tenant
            var existing = await _templateService.GetByIdAsync<FormTemplate>(id, cancellationToken);
            if (existing == null)
            {
                return NotFound($"Template with ID {id} not found");
            }

            if (existing.TenantId != tenantId)
            {
                return Forbid();
            }

            await _templateService.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Template with ID {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting template {TemplateId}", id);
            return StatusCode(500, "An error occurred while deleting the template");
        }
    }
}
