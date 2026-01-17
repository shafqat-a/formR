using System.Text.Json;

namespace FormR.API.DTOs;

public class ControlLibraryDto
{
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public JsonDocument ConfigSchema { get; set; } = null!;
    public JsonDocument DefaultProps { get; set; } = null!;
}
