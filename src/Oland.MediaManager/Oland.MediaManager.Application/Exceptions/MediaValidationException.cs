namespace Oland.MediaManager.Application.Exceptions;

/// <summary>
/// Исключение при ошибке валидации медиа
/// </summary>
public class MediaValidationException : InvalidOperationException
{
    public IReadOnlyList<string> Errors { get; }

    public MediaValidationException(IReadOnlyList<string> errors) 
        : base($"Media validation failed: {string.Join("; ", errors)}")
    {
        Errors = errors;
    }
}