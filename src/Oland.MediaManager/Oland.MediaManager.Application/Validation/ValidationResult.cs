namespace Oland.MediaManager.Application.Validation;

/// <summary>
/// Результат валидации
/// </summary>
public record ValidationResult(IReadOnlyList<string> Errors)
{
    public bool IsValid => Errors.Count == 0;
        
    public static ValidationResult Ok { get; } = new(Array.Empty<string>());
        
    public static ValidationResult Error(params string[] errors) => new(errors);
}