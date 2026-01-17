using System.Text.Json;

namespace FormR.Core.Interfaces;

/// <summary>
/// Validation engine interface for validating form submissions against template rules
/// Implementations will be added in FormR.Core/Validation
/// </summary>
public interface IValidationEngine
{
    /// <summary>
    /// Validates submission data against a form template's validation rules
    /// </summary>
    /// <param name="templateId">The form template ID</param>
    /// <param name="submissionData">JSON object containing form field values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with errors if any</returns>
    Task<ValidationResult> ValidateSubmissionAsync(
        Guid templateId,
        JsonDocument submissionData,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Validation result
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationError> Errors { get; set; } = new();
}

/// <summary>
/// Validation error
/// </summary>
public class ValidationError
{
    public string FieldName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
}
