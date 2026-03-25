using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Odnoklassniki.Exceptions;

namespace Odnoklassniki.Image;

/// <summary>
/// Клиент для работы с изображениями в API Одноклассников.
/// Предоставляет методы для загрузки и скачивания фотографий.
/// </summary>
/// <remarks>
/// Используется в сценариях работы с фотоальбомами, аватарами и другими медиа-ресурсами.
/// Методы класса взаимодействуют напрямую с HTTP-эндпоинтами OK.ru для передачи бинарных данных.
/// </remarks>
public class ImageClient
{
    /// <summary>
    /// Загружает изображение по указанному URL и возвращает его в виде потока данных.
    /// </summary>
    /// <remarks>
    /// Метод создаёт новый экземпляр <see cref="HttpClient"/> для каждого запроса.
    /// Для высоконагруженных сценариев рекомендуется использовать пул клиентов или <see cref="IHttpClientFactory"/>.
    /// Возвращаемый поток должен быть корректно утилизирован вызывающей стороной.
    /// </remarks>
    /// <param name="url">
    /// URL-адрес изображения для загрузки.
    /// Должен быть валидным HTTP/HTTPS-адресом, доступным для анонимного запроса.
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Поток (<see cref="Stream"/>) с бинарными данными изображения.
    /// Содержимое потока соответствует типу MIME, возвращённому сервером.
    /// </returns>
    /// <exception cref="System.Net.Http.HttpRequestException">
    /// Возникает при ошибках сетевого взаимодействия (недоступность хоста, таймаут, DNS-ошибка).
    /// </exception>
    /// <exception cref="System.InvalidOperationException">
    /// Возникает, если URL не является абсолютным или имеет недопустимый формат.
    /// </exception>
    public async Task<Stream> DownloadAsStreamAsync(string url, CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient();
        var responseImage = await httpClient.GetAsync(url, cancellationToken);
        var stream = await responseImage.Content.ReadAsStreamAsync(cancellationToken);
        
        return stream;
    }

    /// <summary>
    /// Загружает изображение на сервер Одноклассников и возвращает токены загруженных фотографий.
    /// </summary>
    /// <remarks>
    /// Метод формирует multipart/form-data запрос с бинарными данными изображения.
    /// Поле формы именуется как <c>pic0</c> (первое фото в серии загрузки).
    /// После успешной загрузки ответ парсится в <see cref="UploadResponse"/> с извлечением токенов.
    /// 
    /// <para><b>Важно:</b></para>
    /// <list type="bullet">
    /// <item>Перед вызовом метода необходимо получить URL для загрузки через метод API <c>photos.getUploadUrl</c>;</item>
    /// <item>После загрузки токены используются для финализации публикации через <c>photos.commit</c>;</item>
    /// <item>Метод создаёт новый <see cref="HttpClient"/> для каждого запроса.</item>
    /// </list>
    /// </remarks>
    /// <param name="requestUri">
    /// URL-адрес эндпоинта для загрузки изображений.
    /// Получается через методы API Одноклассников (например, <c>photos.getUploadUrl</c>).
    /// Обязательный параметр, должен быть валидным URI.
    /// </param>
    /// <param name="stream">
    /// Поток с бинарными данными изображения для загрузки.
    /// Поддерживаемые форматы: JPEG, PNG, GIF (ограничения устанавливаются сервером OK.ru).
    /// Поток должен поддерживать чтение и быть позиционируемым в начале данных.
    /// </param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
    /// <returns>
    /// Словарь с парами <c>ключ → токен</c> для загруженных фотографий.
    /// Ключ соответствует имени поля формы (например, <c>pic0</c>), значение — токену для последующего коммита.
    /// </returns>
    /// <exception cref="Odnoklassniki.Exceptions.OkApiException">
    /// Возникает, если сервер вернул ответ с ошибкой (поле <c>error_code</c> в JSON).
    /// Содержит код и описание ошибки от API Одноклассников.
    /// </exception>
    /// <exception cref="System.Text.Json.JsonException">
    /// Возникает при неудачной десериализации ответа сервера (некорректный JSON или несоответствие схемы).
    /// </exception>
    /// <exception cref="System.Net.Http.HttpRequestException">
    /// Возникает при ошибках сетевого взаимодействия (недоступность хоста, таймаут).
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// Возникает, если <paramref name="stream"/> равен <see langword="null"/>.
    /// </exception>
    [return: NotNull]
    public async Task<IDictionary<string, string>> UploadImageAsync(
        [StringSyntax(StringSyntaxAttribute.Uri)] string? requestUri,
        Stream stream, 
        CancellationToken cancellationToken)
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(stream), $"pic{0}", Guid.NewGuid().ToString());

        using var httpClient = new HttpClient();
        
        var response = await httpClient.PostAsync(requestUri, content, cancellationToken);
        var str = await response.Content.ReadAsStringAsync(cancellationToken);
        OkApiException.ThrowIfError(str);
        
        var uploadResults = JsonSerializer.Deserialize<UploadResponse>(str);
        
        return uploadResults?.Photos.ToDictionary(pair => pair.Key, pair => pair.Value.Token) 
            ?? throw new JsonException(str);
    }
}