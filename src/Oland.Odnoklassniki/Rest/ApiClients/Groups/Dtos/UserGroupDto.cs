namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Dtos;

/// <summary>
/// DTO для представления связи между пользователем и группой.
/// Используется в операциях управления членством, проверки прав доступа и фильтрации по принадлежности.
/// </summary>
public record UserGroupDto
{
    /// <summary>
    /// Уникальный идентификатор группы в системе Одноклассников.
    /// </summary>
    /// <remarks>
    /// Используется для связки с методами API групп (например, <c>groups.getInfo</c>).
    /// Значение формируется сервером OK.ru при создании группы.
    /// </remarks>
    public string GroupId { get; init; }
    
    /// <summary>
    /// Уникальный идентификатор пользователя в системе Одноклассников.
    /// </summary>
    /// <remarks>
    /// Используется для связки с методами API пользователей (например, <c>users.getInfo</c>).
    /// Значение соответствует ID пользователя в социальной сети.
    /// </remarks>
    public string UserId { get; init; }
}