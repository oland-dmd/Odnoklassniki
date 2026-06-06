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

    /// <summary>
    /// Статус (роль) пользователя в группе: администратор, модератор, активный участник и т.д.
    /// </summary>
    /// <remarks>
    /// Заполняется из поля <c>status</c> ответа метода <c>group.getUserGroupsV2</c>.
    /// Если статус не передан сервером или не распознан, значение равно <see cref="GroupStatus.UNKNOWN"/>.
    /// Позволяет получить роль страницы в её группах за один проход, без дополнительного запроса
    /// <c>group.getUserGroupsByIds</c> от имени основного аккаунта.
    /// </remarks>
    public GroupStatus Status { get; init; } = GroupStatus.UNKNOWN;
}