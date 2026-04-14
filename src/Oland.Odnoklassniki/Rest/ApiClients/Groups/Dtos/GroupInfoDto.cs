using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;

/// <summary>
/// DTO для представления основной информации о группе в социальной сети Одноклассники.
/// Используется для передачи данных при получении сведений о группе через методы API.
/// </summary>
public record GroupInfoDto : BaseOkDto
{
    /// <summary>
    /// Уникальный идентификатор группы в системе Одноклассников. Обязательное поле.
    /// </summary>
    /// <remarks>
    /// Значение формируется сервером OK.ru при создании группы.
    /// Используется как ключевой параметр для всех операций с группой (например, в методах <c>groups.getInfo</c>, <c>groups.post</c>).
    /// </remarks>
    [JsonPropertyName("uid")]
    public required string Id { get; init; }

    /// <summary>
    /// Отображаемое название группы. Обязательное поле.
    /// </summary>
    /// <remarks>
    /// Может содержать произвольный текст, включая пробелы и специальные символы.
    /// Максимальная длина ограничена настройками платформы Одноклассников.
    /// </remarks>
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    
    [JsonPropertyName("attrs")]
    public Models.Attributes? Attributes { get; init; }
    
    /// <summary>
    /// Флаг, определяющий возможность добавления новых альбомов в группу текущим пользователем.
    /// </summary>
    /// <value>
    /// <c>true</c> — операция разрешена; <c>false</c> — операция запрещена (недостаточно прав или отключена в настройках группы).
    /// </value>
    /// <remarks>
    /// Значение вычисляется на стороне сервера с учётом роли пользователя в группе и настроек приватности.
    /// Не является справочным полем — зависит от контекста запроса и прав доступа токена.
    /// </remarks>
    public bool AddAlbumAllowed => Attributes?.Flags.Contains("ap") ?? false;
}