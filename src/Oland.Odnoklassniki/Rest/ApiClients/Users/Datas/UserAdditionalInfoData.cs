using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.Users.Datas;

/// <summary>
/// Дополнительная статистическая информация о пользователе OK.ru.
/// Возвращается методом <c>users.getAdditionalInfo</c>.
/// Данные обновляются с задержкой до 24 часов.
/// </summary>
public record UserAdditionalInfoData
{
    /// <summary>Идентификатор пользователя.</summary>
    [JsonPropertyName("user_id")]
    public string? UserId { get; init; }

    /// <summary>Совершал ли пользователь платёж в любом приложении за последний месяц.</summary>
    [JsonPropertyName("app_payer")]
    public bool? AppPayer { get; init; }

    /// <summary>Совершал ли пользователь платёж через OK-валюту за последний месяц.</summary>
    [JsonPropertyName("ok_payer")]
    public bool? OkPayer { get; init; }

    /// <summary>
    /// Диапазон баланса пользователя в OK-валюте.
    /// Возможные значения: <c>0</c>, <c>1-29</c>, <c>30-99</c>, <c>100+</c>.
    /// </summary>
    [JsonPropertyName("balance")]
    public string? Balance { get; init; }

    /// <summary>
    /// Последний способ пополнения баланса.
    /// Возможные значения: <c>NO</c>, <c>SMS</c>, <c>TERMINAL</c>, <c>CARD</c>.
    /// </summary>
    [JsonPropertyName("last_payment")]
    public string? LastPayment { get; init; }

    /// <summary>Был ли пользователь активен в играх за последний месяц.</summary>
    [JsonPropertyName("player")]
    public bool? Player { get; init; }
}
