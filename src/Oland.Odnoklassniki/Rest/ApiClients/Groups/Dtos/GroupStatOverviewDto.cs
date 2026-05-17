using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;

/// <summary>
/// DTO сводной статистики группы за период. Используется в методе <c>group.getStatOverview</c>.
/// Нераспознанные поля ответа доступны через <c>ExtendedData</c>.
/// </summary>
public sealed record GroupStatOverviewDto : BaseOkDto
{
    /// <summary>Охват аудитории (уникальных пользователей).</summary>
    [JsonPropertyName("reach")]
    public long? Reach { get; init; }

    /// <summary>Охват аудитории за предыдущий аналогичный период.</summary>
    [JsonPropertyName("reach_prev")]
    public long? ReachPrev { get; init; }

    /// <summary>Число вовлечений (лайки + комментарии + репосты и т.п.).</summary>
    [JsonPropertyName("engagement")]
    public long? Engagement { get; init; }

    /// <summary>Число вовлечений за предыдущий период.</summary>
    [JsonPropertyName("engagement_prev")]
    public long? EngagementPrev { get; init; }

    /// <summary>Коэффициент вовлечённости (engagement rate).</summary>
    [JsonPropertyName("engagement_rate")]
    public double? EngagementRate { get; init; }

    /// <summary>Коэффициент вовлечённости за предыдущий период.</summary>
    [JsonPropertyName("engagement_rate_prev")]
    public double? EngagementRatePrev { get; init; }

    /// <summary>Количество обратных действий (жалобы, скрытие записей и т.п.).</summary>
    [JsonPropertyName("feedback")]
    public long? Feedback { get; init; }

    /// <summary>Количество обратных действий за предыдущий период.</summary>
    [JsonPropertyName("feedback_prev")]
    public long? FeedbackPrev { get; init; }

    /// <summary>Доля активных пользователей.</summary>
    [JsonPropertyName("active_user_share")]
    public double? ActiveUserShare { get; init; }

    /// <summary>Начало периода статистики (Unix мс).</summary>
    [JsonPropertyName("start_time_ms")]
    public long? StartTimeMs { get; init; }

    /// <summary>Конец периода статистики (Unix мс).</summary>
    [JsonPropertyName("end_time_ms")]
    public long? EndTimeMs { get; init; }

    /// <summary>Список доступных месяцев статистики (Unix мс).</summary>
    [JsonPropertyName("months_ms")]
    public ICollection<long>? MonthsMs { get; init; }
}
