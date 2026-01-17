using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace FormR.Core.Models;

/// <summary>
/// Reference data defining available control types and their configurations
/// </summary>
public class ControlLibrary
{
    /// <summary>
    /// Control type identifier (matches ControlType enum values)
    /// </summary>
    [Key]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;

    [Required]
    public ControlCategory Category { get; set; }

    [Required]
    [MaxLength(50)]
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// JSON Schema defining available configuration properties for this control type
    /// </summary>
    [Required]
    public JsonDocument ConfigSchema { get; set; } = null!;

    /// <summary>
    /// Default configuration when control is added to canvas
    /// </summary>
    [Required]
    public JsonDocument DefaultProps { get; set; } = null!;
}
