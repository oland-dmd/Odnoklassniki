using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.MediaTopics.Responses;

/// <summary>
/// Ответ API Одноклассников на запрос <c>mediatopic.deleteTopic</c>.
/// </summary>
internal sealed record DeleteTopicResponse : BaseOkDto
{
    /// <summary>Флаг успешного удаления.</summary>
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    /// <summary>
    /// Идентификатор для восстановления удалённого топика через <c>mediatopic.restoreTopic</c>.
    /// </summary>
    [JsonPropertyName("restore_id")]
    public string? RestoreId { get; init; }
}
