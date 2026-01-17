using System.ComponentModel.DataAnnotations;

namespace FormR.Core.Models;

public class FormTemplate
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public int Version { get; set; } = 1;

    public Guid? BaseTemplateId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public bool IsDeleted { get; set; } = false;

    [Required]
    public Guid TenantId { get; set; }

    // Navigation properties
    public virtual ICollection<FormControl> Controls { get; set; } = new List<FormControl>();
    public virtual FormTemplate? BaseTemplate { get; set; }
    public virtual ICollection<FormTemplate> DerivedTemplates { get; set; } = new List<FormTemplate>();
}
