using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.MediaTopics.Datas;

/// <summary>
/// Пользователь, проголосовавший за вариант ответа в опросе медиатопика.
/// Используется в ответе <c>mediatopic.getPollAnswerVoters</c>.
/// </summary>
public sealed record PollVoterDto : BaseOkDto
{
    /// <summary>Идентификатор пользователя.</summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; init; }
}
