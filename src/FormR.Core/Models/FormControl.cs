using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace FormR.Core.Models;

public class FormControl
{
    public Guid Id { get; set; }

    [Required]
    public Guid TemplateId { get; set; }

    [Required]
    public ControlType Type { get; set; }

    [Required]
    [MaxLength(200)]
    public string Label { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Placeholder { get; set; }

    public string? DefaultValue { get; set; }

    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// JSON object storing validation rules (regex, min, max, minLength, maxLength, etc.)
    /// </summary>
    public JsonDocument? ValidationRules { get; set; }

    /// <summary>
    /// JSON object storing position: {x, y, width, height, zIndex}
    /// </summary>
    [Required]
    public JsonDocument Position { get; set; } = null!;

    /// <summary>
    /// JSON object storing control-specific configuration (e.g., dropdown options)
    /// </summary>
    public JsonDocument? Properties { get; set; }

    public Guid? ParentControlId { get; set; }

    public int Order { get; set; }

    // Navigation properties
    public virtual FormTemplate Template { get; set; } = null!;
    public virtual FormControl? ParentControl { get; set; }
    public virtual ICollection<FormControl> ChildControls { get; set; } = new List<FormControl>();
}
