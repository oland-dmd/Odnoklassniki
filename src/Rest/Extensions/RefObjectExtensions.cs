using Odnoklassniki.Rest.ApiClients.Discussions.Responses;

namespace Odnoklassniki.Rest.Extensions;

internal static class RefObjectExtensions
{
    /// <summary>
    /// Извлекает идентификатор альбома из массива ссылочных объектов,
    /// отдавая приоритет групповому альбому перед личным.
    /// </summary>
    /// <param name="refObjects">Массив ссылочных объектов из ответа OK API.</param>
    /// <returns>ID альбома или пустая строка, если альбом не найден.</returns>
    public static string ExtractAlbumId(this RefObject[]? refObjects)
    {
        return refObjects?.FirstOrDefault(obj =>
                obj.Type.Equals("GROUP_PHOTO_ALBUM", StringComparison.OrdinalIgnoreCase))?.ID
            ?? refObjects?.FirstOrDefault(obj =>
                obj.Type.Equals("USER_PHOTO_ALBUM", StringComparison.OrdinalIgnoreCase))?.ID
            ?? string.Empty;
    }

    /// <summary>
    /// Извлекает идентификатор группы (сообщества) из массива ссылочных объектов,
    /// возвращённых API «Одноклассники».
    /// </summary>
    /// <param name="refObjects">Массив ссылочных объектов (<see cref="RefObject"/>), 
    /// полученный, например, в составе ответа о обсуждении.</param>
    /// <returns>
    /// Идентификатор группы (например, "123456789"), если среди <paramref name="refObjects"/> 
    /// найден объект с типом <c>"GROUP"</c>; в противном случае — пустая строка.
    /// </returns>
    /// <remarks>
    /// Метод выполняет регистронезависимое сравнение типа объекта.
    /// В API «Одноклассники» группы обычно представлены с типом "GROUP".
    /// </remarks>
    public static string ExtractGroupId(this RefObject[]? refObjects)
    {
        return refObjects?.FirstOrDefault(obj =>
                string.Equals(obj.Type, "GROUP", StringComparison.OrdinalIgnoreCase))?.ID
            ?? string.Empty;
    }
}
