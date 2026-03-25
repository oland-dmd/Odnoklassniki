using System.Text.Json.Serialization;

namespace Odnoklassniki.Rest.ApiClients.Groups.Responses;

/// <summary>
/// Внутренняя модель ответа API Одноклассников при получении информации об участнике группы.
/// Используется для десериализации JSON-ответа от методов работы с участниками групп
/// (например, <c>groups.getMembers</c>, <c>groups.isUserMember</c>).
/// </summary>
/// <remarks>
/// Класс является промежуточной моделью для преобразования ответа API в публичные DTO
/// (например, <see cref="Dtos.GroupUserInfoDto"/>). Не предназначен для прямого использования в бизнес-логике.
/// 
/// <para><b>Пример ответа сервера:</b></para>
/// <code>
/// {
///   "userId": "789012",
///   "status": "ACTIVE"
/// }
/// </code>
/// 
/// <para><b>Возможные значения статуса:</b></para>
/// <list type="bullet">
/// <item><c>ACTIVE</c> — активный участник;</item>
/// <item><c>ADMIN</c> — администратор группы;</item>
/// <item><c>MODERATOR</c> — модератор;</item>
/// <item><c>BLOCKED</c> — заблокирован;</item>
/// <item><c>PASSIVE</c> — пассивный участник;</item>
/// <item><c>MAYBE</c> — статус «возможно» (для мероприятий).</item>
/// </list>
/// Значения маппятся в перечисление <see cref="Dtos.GroupStatus"/> при конвертации в DTO.
/// </remarks>
internal record GroupUserInfoResponse
{
    /// <summary>
    /// Уникальный идентификатор пользователя в системе Одноклассников.
    /// </summary>
    /// <remarks>
    /// Соответствует полю <c>userId</c> в ответе API.
    /// Значение идентифицирует участника группы и используется для связки
    /// с методами API пользователей (например, <c>users.getInfo</c>).
    /// </remarks>
    /// <value>
    /// Строковый идентификатор в формате OK.ru (числовая строка).
    /// Не может быть пустым для валидного ответа.
    /// </value>
    [JsonPropertyName("userId")]
    public string UserId { get; init; }

    /// <summary>
    /// Статус участия пользователя в группе в виде строкового значения.
    /// </summary>
    /// <remarks>
    /// Соответствует полю <c>status</c> в ответе API.
    /// Возвращаемое значение — строка в верхнем регистре (например, <c>"ACTIVE"</c>, <c>"ADMIN"</c>).
    /// При преобразовании в <see cref="Dtos.GroupUserInfoDto"/> значение маппится
    /// в перечисление <see cref="Dtos.GroupStatus"/>.
    /// 
    /// <para><b>Важно:</b></para>
    /// <list type="bullet">
    /// <item>Статус определяется на стороне сервера с учётом роли пользователя;</item>
    /// <item>Неизвестные значения статуса должны обрабатываться как <c>UNKNOWN</c>;</item>
    /// <item>Статус может изменяться динамически (например, при назначении модератором).</item>
    /// </list>
    /// </remarks>
    /// <value>
    /// Строковое представление статуса участника.
    /// Допустимые значения определены спецификацией API Одноклассников.
    /// </value>
    [JsonPropertyName("status")]
    public string Status { get; init; }
}