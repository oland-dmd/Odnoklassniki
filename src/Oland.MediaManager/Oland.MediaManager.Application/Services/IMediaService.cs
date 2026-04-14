using System.ComponentModel.DataAnnotations;
using Oland.MediaManager.Application.Builders;
using Oland.MediaManager.Application.Exceptions;
using ValidationResult = Oland.MediaManager.Application.Validation.ValidationResult;

namespace Oland.MediaManager.Application.Services;

/// <summary>
/// Контракт сервиса для управления медиа-объектами
/// </summary>
public interface IMediaService
{
    /// <summary>
    /// Создаёт коллекцию медиа с помощью конфигурации через builder
    /// </summary>
    MediaCollection Create(Action<MediaCollectionBuilder> configure);

    /// <summary>
    /// Сериализует коллекцию в JSON с предварительной валидацией
    /// </summary>
    /// <exception cref="MediaValidationException">Если валидация не пройдена</exception>
    string Serialize(MediaCollection collection, bool validate = true);

    /// <summary>
    /// Асинхронная сериализация с валидацией
    /// </summary>
    Task<string> SerializeAsync(
        MediaCollection collection, 
        bool validate = true, 
        CancellationToken ct = default);

    /// <summary>
    /// Валидирует коллекцию медиа без сериализации
    /// </summary>
    ValidationResult Validate(MediaCollection collection);

    /// <summary>
    /// Парсит JSON обратно в коллекцию медиа (для десериализации входящих данных)
    /// </summary>
    MediaCollection? Deserialize(string json);

    /// <summary>
    /// Создаёт и сразу сериализует коллекцию в одном вызове (Convenience method)
    /// </summary>
    string CreateAndSerialize(
        Action<MediaCollectionBuilder> configure, 
        bool validate = true);
}