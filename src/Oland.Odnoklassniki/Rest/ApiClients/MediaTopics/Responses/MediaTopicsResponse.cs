using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.MediaTopics.Responses;

/// <summary>
/// Ответ API Одноклассников на запрос <c>mediatopic.getByIds</c>.
/// Содержит коллекцию медиатопиков.
/// </summary>
/// <typeparam name="TDto">Тип DTO элемента, наследующий <see cref="BaseOkDto"/>.</typeparam>
public sealed record MediaTopicsResponse<TDto> where TDto : BaseOkDto
{
    /// <summary>Список медиатопиков.</summary>
    [JsonPropertyName("media_topics")]
    public ICollection<TDto>? MediaTopics { get; init; }
}
