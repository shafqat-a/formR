using FluentValidation;
using FormR.Core.Models;

namespace FormR.Core.Validation;

public class TemplateValidator : AbstractValidator<FormTemplate>
{
    public TemplateValidator()
    {
        RuleFor(t => t.Name)
            .NotEmpty().WithMessage("Template name is required")
            .MaximumLength(200).WithMessage("Template name cannot exceed 200 characters");

        RuleFor(t => t.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
            .When(t => t.Description != null);

        RuleFor(t => t.Version)
            .GreaterThan(0).WithMessage("Version must be greater than 0");

        RuleFor(t => t.TenantId)
            .NotEmpty().WithMessage("TenantId is required");

        RuleFor(t => t.Controls)
            .NotNull().WithMessage("Controls collection cannot be null");

        RuleFor(t => t.BaseTemplateId)
            .Must((template, baseId) => baseId == null || baseId != template.Id)
            .WithMessage("Template cannot be its own base template");
    }
}
