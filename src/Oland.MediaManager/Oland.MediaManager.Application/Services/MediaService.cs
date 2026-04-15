using System.Text.Json;
using System.Text.Json.Serialization;
using Oland.MediaManager.Application.Builders;
using Oland.MediaManager.Application.Exceptions;
using Oland.MediaManager.Application.Validation;
using Oland.MediaManager.Domain.MediaItems;

namespace Oland.MediaManager.Application.Services;

/// <summary>
/// Сервис для управления жизненным циклом медиа-коллекций:
/// создание через билдер, валидация, сериализация в JSON и десериализация.
/// </summary>
public class MediaService : IMediaService
{
    private readonly IMediaValidator? _validator;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="MediaService"/>.
    /// </summary>
    /// <param name="validator">
    /// Опциональный валидатор медиа-контента. Если не передан, валидация пропускается.
    /// </param>
    /// <param name="jsonOptions">
    /// Опции сериализации JSON. По умолчанию используются <see cref="MediaCollection.DefaultOptions"/>.
    /// </param>
    public MediaService(
        IMediaValidator? validator = null, 
        JsonSerializerOptions? jsonOptions = null)
    {
        _validator = validator;
        _jsonOptions = jsonOptions ?? MediaCollection.DefaultOptions;
    }

    /// <summary>
    /// Создаёт новую медиа-коллекцию с помощью конфигурируемого билдера.
    /// </summary>
    /// <param name="configure">
    /// Делегат для настройки <see cref="MediaCollectionBuilder"/>. Обязательный параметр.
    /// </param>
    /// <returns>Готовая коллекция <see cref="MediaCollection"/>.</returns>
    /// <exception cref="ArgumentNullException">Если параметр <paramref name="configure"/> равен null.</exception>
    public MediaCollection Create(Action<MediaCollectionBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        
        var builder = new MediaCollectionBuilder();
        configure(builder);
        return builder.Build();
    }

    /// <summary>
    /// Сериализует медиа-коллекцию в JSON-строку с опциональной валидацией.
    /// </summary>
    /// <param name="collection">Коллекция для сериализации. Обязательный параметр.</param>
    /// <param name="validate">
    /// Флаг выполнения валидации перед сериализацией. По умолчанию — true.
    /// </param>
    /// <returns>JSON-строка в формате { "media": [...] }.</returns>
    /// <exception cref="ArgumentNullException">Если <paramref name="collection"/> равен null.</exception>
    /// <exception cref="MediaValidationException">
    /// Если включена валидация и коллекция не прошла проверку бизнес-правил.
    /// </exception>
    public string Serialize(MediaCollection collection, bool validate = true)
    {
        ArgumentNullException.ThrowIfNull(collection);
        
        if (validate)
        {
            var result = Validate(collection);
            if (!result.IsValid)
                throw new MediaValidationException(result.Errors);
        }

        return JsonSerializer.Serialize(
            new { media = collection.Items }, 
            _jsonOptions);
    }

    /// <summary>
    /// Асинхронная сериализация медиа-коллекции в JSON.
    /// </summary>
    /// <remarks>
    /// В .NET 8+ сериализация поддерживает async, но для обратной совместимости
    /// используется <see cref="Task.Yield"/>. Метод делегирует работу синхронному <see cref="Serialize"/>.
    /// </remarks>
    /// <param name="collection">Коллекция для сериализации.</param>
    /// <param name="validate">Флаг выполнения валидации.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Задача, результатом которой является JSON-строка.</returns>
    public async Task<string> SerializeAsync(
        MediaCollection collection, 
        bool validate = true, 
        CancellationToken ct = default)
    {
        await Task.Yield(); 
        return Serialize(collection, validate);
    }

    /// <summary>
    /// Выполняет валидацию медиа-коллекции согласно бизнес-правилам.
    /// </summary>
    /// <param name="collection">Коллекция для проверки. Обязательный параметр.</param>
    /// <returns>
    /// Результат валидации <see cref="ValidationResult"/>.
    /// Если валидатор не настроен, возвращается успешный результат.
    /// </returns>
    /// <exception cref="ArgumentNullException">Если <paramref name="collection"/> равен null.</exception>
    public ValidationResult Validate(MediaCollection collection)
    {
        ArgumentNullException.ThrowIfNull(collection);
        
        return _validator is null ? ValidationResult.Ok : _validator.Validate(collection);
    }

    /// <summary>
    /// Десериализует JSON-строку обратно в медиа-коллекцию.
    /// </summary>
    /// <param name="json">
    /// JSON-строка в формате { "media": [...] }. Пустые и пробельные значения возвращают null.
    /// </param>
    /// <returns>
    /// Экземпляр <see cref="MediaCollection"/> или null, если входные данные некорректны или пусты.
    /// </returns>
    public MediaCollection? Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        var wrapper = JsonSerializer.Deserialize<MediaWrapper>(json, _jsonOptions);
        return wrapper?.Media is { Count: > 0 } items 
            ? new MediaCollection(items) 
            : null;
    }

    /// <summary>
    /// Комбинированный метод: создаёт коллекцию через билдер и сразу сериализует её в JSON.
    /// </summary>
    /// <param name="configure">Делегат для настройки билдера.</param>
    /// <param name="validate">Флаг выполнения валидации перед сериализацией.</param>
    /// <returns>JSON-строка с сериализованной коллекцией.</returns>
    public string CreateAndSerialize(
        Action<MediaCollectionBuilder> configure, 
        bool validate = true)
    {
        var collection = Create(configure);
        return Serialize(collection, validate);
    }

    /// <summary>
    /// Вспомогательная запись для десериализации JSON-обёртки с полем «media».
    /// </summary>
    /// <param name="Media">Список медиа-элементов, извлечённый из JSON.</param>
    private record MediaWrapper([property: JsonPropertyName("media")] List<MediaItem> Media);
}