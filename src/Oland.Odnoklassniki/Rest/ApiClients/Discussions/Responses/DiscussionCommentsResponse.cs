using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Rest.ApiClients.Discussions.Datas;

namespace Oland.Odnoklassniki.Rest.ApiClients.Discussions.Responses;

/// <summary>
/// Ответ API на запрос <c>discussions.getDiscussionComments</c>.
/// Комментарии расположены в поле <c>commentss</c> (двойная 's' — особенность API OK.ru).
/// </summary>
internal sealed record DiscussionCommentsResponse
{
    /// <summary>Список комментариев обсуждения.</summary>
    [JsonPropertyName("commentss")]
    public ICollection<CommentDetailData>? Comments { get; init; }
}
