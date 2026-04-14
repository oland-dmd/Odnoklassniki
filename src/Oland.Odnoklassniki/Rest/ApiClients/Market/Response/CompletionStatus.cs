using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Market.Response;

public record CompletionStatusResponse : BaseOkDto
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }
}