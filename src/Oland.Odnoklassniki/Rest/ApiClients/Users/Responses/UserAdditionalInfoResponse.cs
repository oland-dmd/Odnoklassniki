using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Rest.ApiClients.Users.Datas;

namespace Oland.Odnoklassniki.Rest.ApiClients.Users.Responses;

/// <summary>
/// Внутренняя модель ответа API на запрос <c>users.getAdditionalInfo</c>.
/// </summary>
internal record UserAdditionalInfoResponse
{
    /// <summary>Список записей с дополнительной информацией по каждому пользователю.</summary>
    [JsonPropertyName("users")]
    public ICollection<UserAdditionalInfoData>? Users { get; init; }
}
