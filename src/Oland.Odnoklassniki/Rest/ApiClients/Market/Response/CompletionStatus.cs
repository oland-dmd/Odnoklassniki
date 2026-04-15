using System.Text.Json.Serialization;
using Oland.Odnoklassniki.Common;

namespace Oland.Odnoklassniki.Rest.ApiClients.Market.Response;

/// <summary>
/// Базовый ответ API Одноклассников на операции изменения состояния (создание, редактирование, удаление).
/// </summary>
/// <remarks>
/// <para>
/// <b>Назначение:</b> унифицированный формат ответа для методов, не возвращающих данные сущности,
/// а только подтверждающих успешность выполнения операции.
/// </para>
/// <para>
/// <b>Используется в методах:</b>
/// <list type="bullet">
/// <item><description><c>market.editCatalog</c>, <c>market.deleteCatalog</c>, <c>market.reorderCatalogs</c></description></item>
/// <item><description><c>market.edit</c>, <c>market.delete</c>, <c>market.pin</c>, <c>market.reorder</c></description></item>
/// </list>
/// </para>
/// <para>
/// <b>Наследование:</b> от <see cref="BaseOkDto"/> обеспечивает совместимость с общей инфраструктурой
/// обработки ответов API Одноклассников (логирование, обработка ошибок, трейсинг).
/// </para>
/// </remarks>
public record CompletionStatusResponse : BaseOkDto
{
    /// <summary>
    /// Флаг успешного выполнения операции.
    /// </summary>
    /// <remarks>
    /// Сериализуется из JSON-поля <c>success</c>.
    /// <c>true</c> — операция завершена без ошибок;
    /// <c>false</c> — операция не выполнена (причины могут быть в коде ошибки базового DTO).
    /// </remarks>
    [JsonPropertyName("success")]
    public bool Success { get; init; }
}