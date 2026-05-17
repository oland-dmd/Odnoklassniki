using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Users.Responses;

/// <summary>
/// Внутренняя модель ответа API на запрос <c>users.getInfoBy</c>.
/// Ответ обёрнут в объект с полем <c>"user"</c>.
/// </summary>
internal record UserGetInfoByResponse<TDto> where TDto : BaseOkDto
{
    /// <summary>Данные пользователя.</summary>
    [JsonPropertyName("user")]
    public TDto? User { get; init; }
}
