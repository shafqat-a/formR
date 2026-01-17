using FluentValidation;
using FormR.Core.Models;

namespace FormR.Core.Validation;

public class ControlValidator : AbstractValidator<FormControl>
{
    public ControlValidator()
    {
        RuleFor(c => c.TemplateId)
            .NotEmpty().WithMessage("TemplateId is required");

        RuleFor(c => c.Type)
            .IsInEnum().WithMessage("Invalid control type");

        RuleFor(c => c.Label)
            .NotEmpty().WithMessage("Control label is required")
            .MaximumLength(200).WithMessage("Label cannot exceed 200 characters");

        RuleFor(c => c.Placeholder)
            .MaximumLength(200).WithMessage("Placeholder cannot exceed 200 characters")
            .When(c => c.Placeholder != null);

        RuleFor(c => c.Position)
            .NotNull().WithMessage("Position is required")
            .Must(BeValidPosition).WithMessage("Position must contain x, y, width, and height properties");

        RuleFor(c => c.Order)
            .GreaterThanOrEqualTo(0).WithMessage("Order must be non-negative");

        RuleFor(c => c.ParentControlId)
            .Must((control, parentId) => parentId == null || parentId != control.Id)
            .WithMessage("Control cannot be its own parent");
    }

    private bool BeValidPosition(System.Text.Json.JsonDocument? position)
    {
        if (position == null) return false;

        try
        {
            var root = position.RootElement;
            return root.TryGetProperty("x", out _) &&
                   root.TryGetProperty("y", out _) &&
                   root.TryGetProperty("width", out _) &&
                   root.TryGetProperty("height", out _);
        }
        catch
        {
            return false;
        }
    }
}
