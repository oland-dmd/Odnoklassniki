using System.Text.Json.Serialization;

namespace Odnoklassniki.Rest.ApiClients.Photos.Response.Album;

/// <summary>
/// Внутренняя модель обёртки ответа API Одноклассников при операциях с альбомом фотографий.
/// Используется для десериализации JSON-ответа от методов создания или получения информации об альбоме.
/// </summary>
/// <remarks>
/// Класс является промежуточной моделью для преобразования ответа API в публичные DTO
/// (например, <see cref="Datas.AlbumData"/>). Не предназначен для прямого использования в бизнес-логике.
/// 
/// <para><b>Пример ответа сервера:</b></para>
/// <code>
/// {
///   "album": {
///     "title": "Фотографии со дня рождения",
///     "aid": "123456",
///     "user_id": "789012",
///     "attrs": { "flags": "1" }
///   }
/// }
/// </code>
/// 
/// <para><b>Сценарии использования:</b></para>
/// <list type="bullet">
/// <item>Обработка ответа от метода <c>photos.createAlbum</c> после создания нового альбома;</item>
/// <item>Обработка ответа от метода <c>photos.getAlbums</c> при получении деталей конкретного альбома;</item>
/// <item>Преобразование в публичную модель <see cref="Datas.AlbumData"/> для передачи в клиентский код.</item>
/// </list>
/// 
/// <para><b>Важно:</b></para>
/// Структура ответа с обёрткой <c>album</c> используется для единичных операций с альбомом.
/// Для списков альбомов может возвращаться массив напрямую без обёртки.
/// </remarks>
internal class AlbumResponse
{
    /// <summary>
    /// Объект с информацией об альбоме фотографий.
    /// </summary>
    /// <remarks>
    /// Соответствует полю <c>album</c> в ответе API Одноклассников.
    /// Содержит десериализованные данные альбома в модели <see cref="AlbumModel"/>.
    /// 
    /// <para><b>Состав данных:</b></para>
    /// <list type="bullet">
    /// <item><see cref="AlbumModel.Title"/> — название альбома;</item>
    /// <item><see cref="AlbumModel.Id"/> — уникальный идентификатор альбома (aid);</item>
    /// <item><see cref="AlbumModel.UserId"/> — идентификатор владельца альбома;</item>
    /// <item><see cref="AlbumModel.Attributes"/> — дополнительные атрибуты и флаги.</item>
    /// </list>
    /// 
    /// <para><b>Важно:</b></para>
    /// <list type="bullet">
    /// <item>Поле может быть <see langword="null"/> при ошибке создания альбома;</item>
    /// <item>При успешной операции всегда содержит валидный объект <see cref="AlbumModel"/>;</item>
    /// <item>Используется для извлечения ID созданного альбома при последующих операциях.</item>
    /// </list>
    /// </remarks>
    /// <value>
    /// Экземпляр <see cref="AlbumModel"/> с данными альбома.
    /// Может быть <see langword="null"/> при неудачном выполнении операции.
    /// </value>
    [JsonPropertyName("album")]
    public AlbumModel Album { get; set; }
}