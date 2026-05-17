using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Rest.ApiClients.Discussions.Datas;

namespace Oland.Odnoklassniki.Rest.ApiClients.Discussions.Responses;

/// <summary>
/// Ответ API на запрос <c>discussions.getComment</c>.
/// Комментарий вложен в поле <c>comment</c>.
/// </summary>
internal sealed record CommentGetResponse
{
    /// <summary>Данные комментария.</summary>
    [JsonPropertyName("comment")]
    public CommentDetailData? Comment { get; init; }
}
