namespace Oland.MediaManager.Application.Exceptions;

/// <summary>
/// Исключение, возникающее при ошибке валидации медиа-контента.
/// Содержит детализированный список нарушений бизнес-правил.
/// </summary>
public class MediaValidationException : InvalidOperationException
{
    /// <summary>
    /// Список сообщений об ошибках валидации. Неизменяемая коллекция.
    /// </summary>
    public IReadOnlyList<string> Errors { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MediaValidationException"/>.
    /// </summary>
    /// <param name="errors">
    /// Коллекция описаний ошибок. Каждая строка должна содержать конкретное нарушение.
    /// </param>
    public MediaValidationException(IReadOnlyList<string> errors) 
        : base($"Media validation failed: {string.Join("; ", errors)}")
    {
        Errors = errors;
    }
}