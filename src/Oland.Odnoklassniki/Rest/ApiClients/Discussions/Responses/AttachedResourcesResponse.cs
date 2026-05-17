using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Rest.ApiClients.Discussions.Datas;

namespace Oland.Odnoklassniki.Rest.ApiClients.Discussions.Responses;

/// <summary>
/// Ответ API на запрос <c>discussions.getAttachedResources</c>.
/// Вложения расположены в поле <c>attachments</c>.
/// </summary>
internal sealed record AttachedResourcesResponse
{
    /// <summary>Список вложенных ресурсов.</summary>
    [JsonPropertyName("attachments")]
    public ICollection<AttachedResourceData>? Attachments { get; init; }
}
