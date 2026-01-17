using System.Text.Json;

namespace FormR.API.DTOs;

public class TemplateListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Version { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public int ControlCount { get; set; }
}

public class TemplateDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Version { get; set; }
    public Guid? BaseTemplateId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public List<ControlDto> Controls { get; set; } = new();
}

public class CreateTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<CreateControlDto> Controls { get; set; } = new();
}

public class UpdateTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Version { get; set; }
    public List<UpdateControlDto> Controls { get; set; } = new();
}

public class ControlDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Placeholder { get; set; }
    public string? DefaultValue { get; set; }
    public bool IsRequired { get; set; }
    public JsonDocument? ValidationRules { get; set; }
    public JsonDocument Position { get; set; } = null!;
    public JsonDocument? Properties { get; set; }
    public Guid? ParentControlId { get; set; }
    public int Order { get; set; }
}

public class CreateControlDto
{
    public string Type { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Placeholder { get; set; }
    public string? DefaultValue { get; set; }
    public bool IsRequired { get; set; }
    public JsonDocument? ValidationRules { get; set; }
    public JsonDocument Position { get; set; } = null!;
    public JsonDocument? Properties { get; set; }
    public Guid? ParentControlId { get; set; }
    public int Order { get; set; }
}

public class UpdateControlDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Placeholder { get; set; }
    public string? DefaultValue { get; set; }
    public bool IsRequired { get; set; }
    public JsonDocument? ValidationRules { get; set; }
    public JsonDocument Position { get; set; } = null!;
    public JsonDocument? Properties { get; set; }
    public Guid? ParentControlId { get; set; }
    public int Order { get; set; }
}
