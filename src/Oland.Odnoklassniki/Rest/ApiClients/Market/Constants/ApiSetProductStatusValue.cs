namespace Oland.Odnoklassniki.Rest.ApiClients.Market.Constants;

/// <summary>
/// Значения параметра <c>product_status</c> метода <c>market.setStatus</c> (заглавные — enum-имена
/// на стороне OK, в отличие от строчных значений поля <c>status</c> в ответах чтения,
/// см. <see cref="ApiProductStatus"/>). Метод строгий: попытка установить статус, совпадающий
/// с текущим (например ACTIVE -> ACTIVE), возвращает ошибку <c>mediaTopic.editAdvert.notFound</c>,
/// а не идемпотентный успех — подтверждено живым вызовом API.
/// </summary>
public static class ApiSetProductStatusValue
{
    public const string Active = "ACTIVE";
    public const string Closed = "CLOSED";
    public const string Sold = "SOLD";
}
