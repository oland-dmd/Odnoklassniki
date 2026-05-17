using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;

/// <summary>
/// DTO демографической статистики аудитории группы. Используется в методе <c>group.getStatPeople</c>.
/// Нераспознанные поля доступны через <c>ExtendedData</c>.
/// </summary>
public sealed record GroupStatPeopleDto : BaseOkDto
{
    /// <summary>Распределение аудитории по городам.</summary>
    [JsonPropertyName("cities")]
    public ICollection<StatDemographicEntry>? Cities { get; init; }

    /// <summary>Распределение аудитории по странам.</summary>
    [JsonPropertyName("countries")]
    public ICollection<StatDemographicEntry>? Countries { get; init; }

    /// <summary>Распределение аудитории-женщин по возрасту.</summary>
    [JsonPropertyName("demography_female")]
    public ICollection<StatDemographicEntry>? DemographyFemale { get; init; }

    /// <summary>Распределение аудитории-мужчин по возрасту.</summary>
    [JsonPropertyName("demography_male")]
    public ICollection<StatDemographicEntry>? DemographyMale { get; init; }

    /// <summary>Источники перехода на страницу группы.</summary>
    [JsonPropertyName("references")]
    public ICollection<StatDemographicEntry>? References { get; init; }
}
