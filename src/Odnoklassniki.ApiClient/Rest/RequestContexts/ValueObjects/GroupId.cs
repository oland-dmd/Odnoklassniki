namespace Odnoklassniki.Rest.RequestContexts.ValueObjects;

/// <summary>
/// Типизированный идентификатор группы в социальной сети Одноклассники (OK.ru).
/// Представляет собой value object для безопасной передачи и валидации идентификаторов групп
/// в методах API, связанных с управлением группами и проверкой членства пользователей.
/// </summary>
/// <remarks>
/// Структура является неизменяемой (immutable) и обеспечивает типобезопасность при работе
/// с идентификаторами групп, предотвращая случайную передачу невалидных значений.
/// <list type="bullet">
/// <item><description>Значение проходит валидацию при создании: пустые строки и <see langword="null"/> не допускаются.</description></item>
/// <item><description>Формат идентификатора определяется платформой OK.ru (обычно строковое числовое значение).</description></item>
/// </list>
/// </remarks>
public record GroupId
{
    /// <summary>
    /// Строковое представление идентификатора группы в формате OK.ru.
    /// </summary>
    /// <remarks>
    /// Содержит валидированное значение, переданное при создании экземпляра.
    /// Не может быть пустой строкой или содержать только пробельные символы.
    /// Формат значения должен соответствовать спецификации API Одноклассников.
    /// </remarks>
    public string Value { get; }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="GroupId"/> с проверкой валидности входных данных.
    /// </summary>
    /// <param name="value">
    /// Идентификатор группы в формате OK.ru. Обязательное поле.
    /// Должен быть непустой строкой, содержащей допустимое значение идентификатора.
    /// </param>
    /// <exception cref="System.ArgumentException">
    /// Если параметр <paramref name="value"/> является <see langword="null"/>,
    /// пустой строкой или содержит только пробельные символы.
    /// </exception>
    public GroupId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Group ID cannot be empty", nameof(value));
        }
        
        Value = value;
    }
}