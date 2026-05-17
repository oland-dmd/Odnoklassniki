using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;

/// <summary>
/// Одна точка временного ряда статистики.
/// Используется в <see cref="GroupStatTrendsDto"/> для каждого показателя.
/// </summary>
public sealed record TrendDataPoint
{
    /// <summary>Дата точки в миллисекундах Unix epoch.</summary>
    [JsonPropertyName("time")]
    public long? Time { get; init; }

    /// <summary>Значение показателя в данной точке.</summary>
    [JsonPropertyName("value")]
    public long? Value { get; init; }
}
