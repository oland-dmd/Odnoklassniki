using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;

/// <summary>
/// Одна запись демографической разбивки аудитории группы.
/// Используется в <see cref="GroupStatPeopleDto"/> для полей
/// <c>cities</c>, <c>countries</c>, <c>demography_female</c>, <c>demography_male</c>, <c>references</c>.
/// </summary>
public sealed record StatDemographicEntry
{
    /// <summary>Метка категории (например, «Мужчины», «18-25», «Россия»).</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>Доля данной категории в процентах от общей аудитории.</summary>
    [JsonPropertyName("percentage")]
    public double? Percentage { get; init; }

    /// <summary>Абсолютное количество пользователей в данной категории.</summary>
    [JsonPropertyName("value")]
    public long? Value { get; init; }
}
