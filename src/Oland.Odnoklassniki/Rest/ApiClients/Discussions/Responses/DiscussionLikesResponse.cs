using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Rest.ApiClients.Photos.Datas;

namespace Oland.Odnoklassniki.Rest.ApiClients.Discussions.Responses;

/// <summary>
/// Ответ API на запросы <c>discussions.getDiscussionLikes</c> и <c>discussions.getCommentLikes</c>.
/// Содержит постраничный список пользователей, поставивших лайк.
/// </summary>
internal sealed record DiscussionLikesResponse
{
    /// <summary>Маркер для получения следующей страницы.</summary>
    [JsonPropertyName("anchor")]
    public string? Anchor { get; init; }

    /// <summary>Пользователи, поставившие лайк. Дополнительные поля доступны через <c>ExtendedData</c> каждого элемента.</summary>
    [JsonPropertyName("users")]
    public ICollection<UserLikeDto>? Users { get; init; }
}
