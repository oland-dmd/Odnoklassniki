namespace Oland.Odnoklassniki.Rest.ApiClients.Groups.Extensions;

/// <summary>
/// Класс расширений для упрощения работы с параметрами запросов API групп Одноклассников.
/// Предоставляет методы-обёртки для добавления часто используемых параметров в <see cref="RestParameters"/>.
/// </summary>
/// <remarks>
/// Методы класса инкапсулируют логику формирования параметров согласно спецификации API OK.ru.
/// Используются при построении запросов к методам групп (<c>groups.getMembers</c>, <c>groups.getInfo</c> и др.).
/// Все методы поддерживают Fluent Interface и возвращают модифицированный экземпляр параметров.
/// </remarks>
internal static class RestParametersExtensions
{
    /// <summary>
    /// Добавляет идентификаторы групп в параметры запроса.
    /// </summary>
    /// <remarks>
    /// Формирует параметр <c>uids</c> со списком ID групп, разделённых запятыми.
    /// Пример: <c>groupIds = ["123", "456"]</c> → <c>uids=123,456</c>.
    /// 
    /// <para><b>Важно:</b></para>
    /// Параметр <c>uids</c> используется в API Одноклассников как для пользователей, так и для групп.
    /// Контекст определяется методом API, к которому применяется запрос.
    /// </remarks>
    /// <param name="parameters">Экземпляр параметров запроса для модификации.</param>
    /// <param name="groupIds">
    /// Коллекция идентификаторов групп в формате OK.ru (строковые ID).
    /// Может содержать один или несколько идентификаторов.
    /// </param>
    /// <returns>Тот же экземпляр <see cref="RestParameters"/> с добавленным параметром <c>uids</c>.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// Возникает, если <paramref name="parameters"/> или <paramref name="groupIds"/> равен <see langword="null"/>.
    /// </exception>
    public static RestParameters InsertGroups(this RestParameters parameters, params IEnumerable<string> groupIds)
    {
        parameters.InsertCustomParameter("uids", string.Join(',', groupIds));
     
        return parameters;
    }

    /// <summary>
    /// Добавляет идентификаторы пользователей в параметры запроса.
    /// </summary>
    /// <remarks>
    /// Формирует параметр <c>uids</c> со списком ID пользователей, разделённых запятыми.
    /// Пример: <c>userIds = ["123", "456"]</c> → <c>uids=123,456</c>.
    /// 
    /// <para><b>Примечание:</b></para>
    /// Метод использует тот же параметр <c>uids</c>, что и <see cref="InsertGroups"/>.
    /// При одновременном вызове обоих методов последнее значение перезапишет предыдущее.
    /// Для запросов, требующих разделения пользователей и групп, используйте явное добавление параметров.
    /// </remarks>
    /// <param name="parameters">Экземпляр параметров запроса для модификации.</param>
    /// <param name="userIds">
    /// Коллекция идентификаторов пользователей в формате OK.ru (строковые ID).
    /// Может содержать один или несколько идентификаторов.
    /// </param>
    /// <returns>Тот же экземпляр <see cref="RestParameters"/> с добавленным параметром <c>uids</c>.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// Возникает, если <paramref name="parameters"/> или <paramref name="userIds"/> равен <see langword="null"/>.
    /// </exception>
    public static RestParameters InsertUsers(this RestParameters parameters, params IEnumerable<string> userIds)
    {
        parameters.InsertCustomParameter("uids", string.Join(',', userIds));

        return parameters;
    }

    /// <summary>
    /// Добавляет идентификатор корневой группы в параметры запроса.
    /// </summary>
    /// <remarks>
    /// Формирует параметр <c>group_id</c> с идентификатором целевой группы.
    /// Используется в методах, требующих указания конкретной группы для операции
    /// (например, <c>groups.getMembers</c>, <c>groups.getInfo</c>, <c>photos.getAlbums</c>).
    /// 
    /// <para><b>Пример использования:</b></para>
    /// <code>
    /// parameters.InsertRootGroupId("123456");
    /// // Результат: group_id=123456
    /// </code>
    /// </remarks>
    /// <param name="parameters">Экземпляр параметров запроса для модификации.</param>
    /// <param name="groupId">
    /// Идентификатор группы в формате OK.ru (строковый ID).
    /// Должен соответствовать существующей группе в системе.
    /// </param>
    /// <returns>Тот же экземпляр <see cref="RestParameters"/> с добавленным параметром <c>group_id</c>.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// Возникает, если <paramref name="parameters"/> равен <see langword="null"/>.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// Возникает, если <paramref name="groupId"/> пуст или содержит недопустимые символы.
    /// </exception>
    public static RestParameters InsertRootGroupId(this RestParameters parameters, string groupId)
    {
        parameters.InsertCustomParameter("group_id", groupId);

        return parameters;
    }
}