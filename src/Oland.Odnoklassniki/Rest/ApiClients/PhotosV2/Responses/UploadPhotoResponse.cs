using System.Text.Json.Serialization;

namespace Oland.Odnoklassniki.Rest.ApiClients.PhotosV2.Responses;

/// <summary>
/// Внутренняя модель ответа API Одноклассников (версия 2) при получении URL для загрузки фотографий.
/// Используется для десериализации JSON-ответа от метода <c>photos.getUploadUrl</c>.
/// </summary>
/// <remarks>
/// Класс является промежуточной моделью для преобразования ответа API в публичные DTO
/// (например, <see cref="Datas.UploadUrlData"/>). Не предназначен для прямого использования в бизнес-логике.
/// 
/// <para><b>Пример ответа сервера:</b></para>
/// <code>
/// {
///   "photo_ids": ["123456", "789012"],
///   "upload_url": "https://www.ok.ru/up?act=upload&token=abc123"
/// }
/// </code>
/// 
/// <para><b>Полный цикл загрузки фотографии:</b></para>
/// <list type="number">
/// <item>Вызвать <c>photos.getUploadUrl</c> для получения <see cref="UploadPhotoResponse"/>;</item>
/// <item>Загрузить изображение через <see cref="UploadUrl"/> методом POST (multipart/form-data);</item>
/// <item>Получить токены из ответа сервера загрузки;</item>
/// <item>Вызвать <c>photos.commit</c> с токенами для финализации публикации;</item>
/// <item>Получить идентификаторы опубликованных фотографий из <see cref="CommitResponse"/>.</item>
/// </list>
/// 
/// <para><b>Важно:</b></para>
/// <list type="bullet">
/// <item>URL загрузки действителен ограниченное время (рекомендуется немедленная загрузка);</item>
/// <item>PhotoIds — это предварительно зарезервированные идентификаторы для загружаемых фотографий;</item>
/// <item>Модель используется только в API версии 2 (PhotosV2).</item>
/// </list>
/// </remarks>
public record UploadPhotoResponse
{
    /// <summary>
    /// Массив идентификаторов фотографий, зарезервированных для загрузки. Обязательное поле.
    /// </summary>
    /// <remarks>
    /// Соответствует полю <c>photo_ids</c> в ответе API <c>photos.getUploadUrl</c>.
    /// Содержит предварительно выделенные идентификаторы для каждой фотографии, которая будет загружена.
    /// 
    /// <para><b>Особенности:</b></para>
    /// <list type="bullet">
    /// <item>Количество ID соответствует запрошенному количеству фотографий в параметре <c>count</c>;</item>
    /// <item>Идентификаторы резервируются сервером до фактической загрузки изображений;</item>
    /// <item>При успешной загрузке и коммите эти ID становятся постоянными идентификаторами фотографий;</item>
    /// <item>При отмене загрузки или ошибке коммита ID освобождаются;</item>
    /// <item>Порядок ID соответствует порядку загрузки изображений в multipart-запросе.</item>
    /// </list>
    /// 
    /// <para><b>Пример значения:</b></para>
    /// <c>["123456", "789012", "345678"]</c> — для загрузки трёх фотографий.
    /// </remarks>
    /// <value>
    /// Массив строковых идентификаторов в формате OK.ru (числовые строки).
    /// Не может быть <see langword="null"/>, но может быть пустым при ошибке резервирования.
    /// </value>
    [JsonPropertyName("photo_ids")]
    public required string[] PhotoIds { get; init; }

    /// <summary>
    /// Временный URL-адрес эндпоинта для загрузки бинарных данных фотографий. Обязательное поле.
    /// </summary>
    /// <remarks>
    /// Соответствует полю <c>upload_url</c> в ответе API <c>photos.getUploadUrl</c>.
    /// Содержит прямой HTTPS-адрес на сервер загрузки изображений Одноклассников.
    /// 
    /// <para><b>Формат запроса загрузки:</b></para>
    /// <list type="bullet">
    /// <item>Метод: <c>POST</c>;</item>
    /// <item>Content-Type: <c>multipart/form-data</c>;</item>
    /// <item>Имя поля: <c>pic0</c>, <c>pic1</c>, ... (по количеству фотографий);</item>
    /// <item>Значение поля: бинарные данные изображения (Stream).</item>
    /// </list>
    /// 
    /// <para><b>Важно:</b></para>
    /// <list type="bullet">
    /// <item>URL действителен ограниченное время (обычно несколько минут);</item>
    /// <item>Повторное использование URL после успешной загрузки невозможно;</item>
    /// <item>При ошибке загрузки необходимо запросить новый URL;</item>
    /// <item>URL содержит параметры безопасности (токен, подпись, срок действия);</item>
    /// <item>Не рекомендуется логировать URL из-за наличия чувствительных токенов.</item>
    /// </list>
    /// 
    /// <para><b>Пример значения:</b></para>
    /// <c>https://www.ok.ru/up?act=upload&amp;token=abc123&amp;expires=1234567890</c>
    /// </remarks>
    /// <value>
    /// Строковый URL-адрес в формате HTTPS. Не может быть пустым или <see langword="null"/>.
    /// </value>
    [JsonPropertyName("upload_url")]
    public required string UploadUrl { get; init; }
}