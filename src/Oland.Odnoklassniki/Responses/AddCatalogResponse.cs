using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Responses;

public class AddEntityResponse : DynamicIdEntity
{
    [JsonPropertyName("success")]
    public bool Success {get; set;}
}