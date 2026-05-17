using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.Friends.Datas;

/// <summary>
/// DTO с информацией о дне рождения друга OK.ru.
/// Возвращается методом <c>friends.getBirthdays</c>.
/// </summary>
public record UserBirthdayDto
{
    /// <summary>Уникальный идентификатор пользователя.</summary>
    [JsonPropertyName("uid")]
    public string? Uid { get; init; }

    /// <summary>Дата рождения в формате Date (строка).</summary>
    [JsonPropertyName("date")]
    public string? Date { get; init; }
}
