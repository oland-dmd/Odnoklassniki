using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;

/// <summary>
/// DTO статистики одного поста (топика) группы.
/// Используется в методах <c>group.getStatTopic</c> и <c>group.getStatTopics</c>.
/// Нераспознанные поля доступны через <c>ExtendedData</c>.
/// </summary>
public sealed record GroupStatTopicDto : BaseOkDto
{
    /// <summary>Идентификатор топика.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>Дата создания топика (Unix мс).</summary>
    [JsonPropertyName("created_ms")]
    public long? CreatedMs { get; init; }

    /// <summary>Просмотры топика.</summary>
    [JsonPropertyName("views")]
    public long? Views { get; init; }

    /// <summary>Отрисовки (показы) топика — включают просмотры в ленте.</summary>
    [JsonPropertyName("renderings")]
    public long? Renderings { get; init; }

    /// <summary>Охват аудитории (уникальных пользователей).</summary>
    [JsonPropertyName("reach")]
    public long? Reach { get; init; }

    /// <summary>Органический охват.</summary>
    [JsonPropertyName("reach_own")]
    public long? ReachOwn { get; init; }

    /// <summary>Виральный охват (от репостов и лайков).</summary>
    [JsonPropertyName("reach_earned")]
    public long? ReachEarned { get; init; }

    /// <summary>Количество лайков.</summary>
    [JsonPropertyName("likes")]
    public long? Likes { get; init; }

    /// <summary>Количество комментариев.</summary>
    [JsonPropertyName("comments")]
    public long? Comments { get; init; }

    /// <summary>Количество репостов.</summary>
    [JsonPropertyName("reshares")]
    public long? Reshares { get; init; }

    /// <summary>Вовлечённость (лайки + комментарии + репосты и т.п.).</summary>
    [JsonPropertyName("engagement")]
    public long? Engagement { get; init; }

    /// <summary>Обратные реакции (жалобы, скрытие из ленты).</summary>
    [JsonPropertyName("feedback")]
    public long? Feedback { get; init; }

    /// <summary>Переходы по ссылке из топика.</summary>
    [JsonPropertyName("link_clicks")]
    public long? LinkClicks { get; init; }

    /// <summary>Воспроизведения видео.</summary>
    [JsonPropertyName("video_plays")]
    public long? VideoPlays { get; init; }

    /// <summary>Открытия полного содержимого топика.</summary>
    [JsonPropertyName("content_opens")]
    public long? ContentOpens { get; init; }
}
