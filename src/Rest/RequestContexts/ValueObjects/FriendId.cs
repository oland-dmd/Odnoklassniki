namespace Odnoklassniki.Rest.RequestContexts.ValueObjects;

/// <summary>
/// Типизированный идентификатор друга в социальной сети Одноклассники (OK.ru).
/// Представляет собой value object для безопасной передачи и валидации идентификаторов пользователей
/// в методах API, связанных с управлением списком друзей.
/// </summary>
/// <remarks>
/// Структура является неизменяемой (immutable) и обеспечивает типобезопасность при работе
/// с идентификаторами друзей, предотвращая случайную передачу невалидных значений.
/// <list type="bullet">
/// <item><description>Значение проходит валидацию при создании: пустые строки и <see langword="null"/> не допускаются.</description></item>
/// <item><description>Формат идентификатора определяется платформой OK.ru (обычно строковое числовое значение).</description></item>
/// <item><description>Используется в методах <see cref="IFriendsApiClient"/> и связанных интерфейсах.</description></item>
/// </list>
/// </remarks>
public readonly record struct FriendId
{
    /// <summary>
    /// Строковое представление идентификатора друга в формате OK.ru.
    /// </summary>
    /// <remarks>
    /// Содержит валидированное значение, переданное при создании экземпляра.
    /// Не может быть пустой строкой или содержать только пробельные символы.
    /// Формат значения должен соответствовать спецификации API Одноклассников.
    /// </remarks>
    public string Value { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="FriendId"/> с проверкой валидности входных данных.
    /// </summary>
    /// <param name="value">
    /// Идентификатор друга в формате OK.ru. Обязательное поле.
    /// Должен быть непустой строкой, содержащей допустимое значение идентификатора.
    /// </param>
    /// <exception cref="System.ArgumentException">
    /// Если параметр <paramref name="value"/> является <see langword="null"/>,
    /// пустой строкой или содержит только пробельные символы.
    /// </exception>
    public FriendId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Friend ID cannot be empty", nameof(value));
        }
        
        Value = value;
    }
}