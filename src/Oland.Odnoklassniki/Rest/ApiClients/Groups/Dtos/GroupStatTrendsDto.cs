using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;

/// <summary>
/// DTO трендов (временных рядов) статистики группы. Используется в методе <c>group.getStatTrends</c>.
/// Каждое свойство — массив точек <see cref="TrendDataPoint"/> (поля <c>time</c> и <c>value</c>) для одного показателя.
/// Нераспознанные показатели доступны через <c>ExtendedData</c>.
/// </summary>
public sealed record GroupStatTrendsDto : BaseOkDto
{
    /// <summary>Динамика охвата аудитории.</summary>
    [JsonPropertyName("reach")]
    public ICollection<TrendDataPoint>? Reach { get; init; }

    /// <summary>Динамика органического охвата.</summary>
    [JsonPropertyName("reach_own")]
    public ICollection<TrendDataPoint>? ReachOwn { get; init; }

    /// <summary>Динамика вирального охвата.</summary>
    [JsonPropertyName("reach_earned")]
    public ICollection<TrendDataPoint>? ReachEarned { get; init; }

    /// <summary>Динамика отрисовок (показов) материалов группы.</summary>
    [JsonPropertyName("renderings")]
    public ICollection<TrendDataPoint>? Renderings { get; init; }

    /// <summary>Динамика количества лайков.</summary>
    [JsonPropertyName("likes")]
    public ICollection<TrendDataPoint>? Likes { get; init; }

    /// <summary>Динамика количества комментариев.</summary>
    [JsonPropertyName("comments")]
    public ICollection<TrendDataPoint>? Comments { get; init; }

    /// <summary>Динамика количества репостов.</summary>
    [JsonPropertyName("reshares")]
    public ICollection<TrendDataPoint>? Reshares { get; init; }

    /// <summary>Динамика вовлечённости.</summary>
    [JsonPropertyName("engagement")]
    public ICollection<TrendDataPoint>? Engagement { get; init; }

    /// <summary>Динамика обратных реакций (жалобы, скрытие из ленты).</summary>
    [JsonPropertyName("feedback")]
    public ICollection<TrendDataPoint>? Feedback { get; init; }

    /// <summary>Динамика посещений страницы группы.</summary>
    [JsonPropertyName("page_visits")]
    public ICollection<TrendDataPoint>? PageVisits { get; init; }

    /// <summary>Динамика общего количества участников группы.</summary>
    [JsonPropertyName("members_count")]
    public ICollection<TrendDataPoint>? MembersCount { get; init; }

    /// <summary>Динамика новых вступивших участников.</summary>
    [JsonPropertyName("new_members")]
    public ICollection<TrendDataPoint>? NewMembers { get; init; }

    /// <summary>Динамика покинувших группу участников.</summary>
    [JsonPropertyName("left_members")]
    public ICollection<TrendDataPoint>? LeftMembers { get; init; }

    /// <summary>Динамика переходов по ссылкам в постах.</summary>
    [JsonPropertyName("link_clicks")]
    public ICollection<TrendDataPoint>? LinkClicks { get; init; }

    /// <summary>Динамика воспроизведений видео.</summary>
    [JsonPropertyName("video_plays")]
    public ICollection<TrendDataPoint>? VideoPlays { get; init; }
}
