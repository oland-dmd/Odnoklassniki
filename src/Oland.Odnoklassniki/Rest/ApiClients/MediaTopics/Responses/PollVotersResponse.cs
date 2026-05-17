using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Rest.ApiClients.MediaTopics.Datas;

namespace Oland.Odnoklassniki.Rest.ApiClients.MediaTopics.Responses;

/// <summary>
/// Ответ API Одноклассников на запрос <c>mediatopic.getPollAnswerVoters</c>.
/// </summary>
public sealed record PollVotersResponse
{
    /// <summary>Курсор для получения следующей страницы.</summary>
    [JsonPropertyName("anchor")]
    public string? Anchor { get; init; }

    /// <summary>Флаг наличия дополнительных страниц.</summary>
    [JsonPropertyName("has_more")]
    public bool HasMore { get; init; }

    /// <summary>Общее количество проголосовавших.</summary>
    [JsonPropertyName("total_count")]
    public int TotalCount { get; init; }

    /// <summary>
    /// Сущности ответа, содержащие профили проголосовавших пользователей.
    /// Пользователи находятся в <c>entities.users</c>.
    /// </summary>
    [JsonPropertyName("entities")]
    public PollVotersEntities? Entities { get; init; }
}

/// <summary>
/// Контейнер сущностей ответа <c>mediatopic.getPollAnswerVoters</c>.
/// </summary>
public sealed record PollVotersEntities
{
    /// <summary>Профили проголосовавших пользователей.</summary>
    [JsonPropertyName("users")]
    public ICollection<PollVoterDto>? Users { get; init; }
}
