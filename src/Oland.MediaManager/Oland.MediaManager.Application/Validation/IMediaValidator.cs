using Oland.MediaManager.Application.Builders;

namespace Oland.MediaManager.Application.Validation;

/// <summary>
/// Сервис валидации медиа-контента
/// </summary>
public interface IMediaValidator
{
    ValidationResult Validate(MediaCollection collection);
}