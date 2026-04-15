namespace Oland.Odnoklassniki.Rest.ApiClients.Market.Models;

/// <summary>
/// Модель данных товара для маркетплейса Одноклассников.
/// </summary>
/// <remarks>
/// <para>
/// Используется в методах <see cref="IMarketProductsApiClient.AddAsync"/> и
/// <see cref="IMarketProductsApiClient.EditAsync"/> для передачи данных товара.
/// </para>
/// <para>
/// <b>Бизнес-правила:</b>
/// <list type="bullet">
/// <item><description>Товар должен содержать либо описание, либо хотя бы одну фотографию.</description></item>
/// <item><description>Все фотографии в коллекции должны проходить валидацию (<see cref="PhotoModel.IsValid"/>).</description></item>
/// <item><description>Цена и срок жизни не могут быть отрицательными.</description></item>
/// </list>
/// </para>
/// <para>
/// <b>Сериализация:</b> данные модели преобразуются в JSON-attachment через <see cref="IMediaService"/>
/// перед отправкой в API Одноклассников.
/// </para>
/// </remarks>
public record ProductModel
{
    /// <summary>
    /// Название товара. Обязательное поле.
    /// </summary>
    /// <remarks>
    /// Отображается в заголовке карточки товара. Рекомендуемая длина — до 100 символов.
    /// Не должен содержать только пробельные символы.
    /// </remarks>
    public required string Title { get; init; }
    
    /// <summary>
    /// Текстовое описание товара. Необязательное поле.
    /// </summary>
    /// <remarks>
    /// Поддерживает простой текст или ограниченную разметку (зависит от настроек платформы).
    /// Может быть пустым, если товар содержит фотографии (<see cref="Photos"/>).
    /// </remarks>
    public string? Description { get; init; }
    
    /// <summary>
    /// Коллекция фотографий товара. Необязательное поле.
    /// </summary>
    /// <remarks>
    /// Каждая фотография представлена экземпляром <see cref="PhotoModel"/> с токеном.
    /// Максимальное количество фото ограничено настройками маркетплейса (обычно до 10).
    /// Порядок элементов в коллекции определяет порядок отображения в карточке товара.
    /// </remarks>
    public ICollection<PhotoModel>? Photos { get; init; }
    
    /// <summary>
    /// Срок актуальности предложения в днях. Необязательное поле, значение по умолчанию — 0 (бессрочно).
    /// </summary>
    /// <remarks>
    /// После истечения срока товар может быть автоматически скрыт из выдачи или помечен как неактуальный.
    /// Допустимые значения: от 0 до 365 (ограничение может зависеть от типа аккаунта).
    /// </remarks>
    public int LifetimePerDays { get; init; }
    
    /// <summary>
    /// Цена товара. Обязательное поле.
    /// </summary>
    /// <remarks>
    /// Указывается в десятичном формате с учётом минорной единицы валюты
    /// (например, 99.90 для 99 рублей 90 копеек).
    /// Допустимые значения: от 0 включительно. Нулевая цена может использоваться для бесплатных товаров.
    /// </remarks>
    public required decimal Price { get; init; }

    /// <summary>
    /// Код валюты цены в формате ISO 4217 (например, «RUB», «USD», «KZT»). Необязательное поле.
    /// </summary>
    /// <remarks>
    /// Если не указан, используется валюта по умолчанию для региона группы или аккаунта.
    /// Список поддерживаемых валют определяется настройками маркетплейса Одноклассников.
    /// </remarks>
    public string? Currency { get; init; }
    
    /// <summary>
    /// Партнёрская ссылка для перехода после покупки/просмотра товара. Необязательное поле.
    /// </summary>
    /// <remarks>
    /// Должна содержать валидный URL с протоколом (http/https).
    /// Используется для трекинга реферальных переходов в партнёрских программах.
    /// </remarks>
    public string? PartnerLink { get; init; }
    
    /// <summary>
    /// Проверяет валидность модели товара согласно бизнес-правилам платформы.
    /// </summary>
    /// <value>
    /// <c>true</c>, если выполнены все условия:
    /// <list type="bullet">
    /// <item><description><see cref="Title"/> не пуст и не состоит из пробелов;</description></item>
    /// <item><description><see cref="Description"/> не пуст <b>или</b> коллекция <see cref="Photos"/> содержит хотя бы один элемент;</description></item>
    /// <item><description>Все элементы <see cref="Photos"/> проходят валидацию (<see cref="PhotoModel.IsValid"/>);</description></item>
    /// <item><description><see cref="LifetimePerDays"/> >= 0;</description></item>
    /// <item><description><see cref="Price"/> >= 0.</description></item>
    /// </list>
    /// Иначе <c>false</c>.
    /// </value>
    /// <remarks>
    /// Валидация выполняется на стороне клиента перед отправкой запроса в API.
    /// Сервер Одноклассников также выполняет собственную проверку, которая может быть строже.
    /// </remarks>
    public bool IsValid => 
        (!string.IsNullOrWhiteSpace(Description) || Photos?.Count > 0)
        && (Photos?.All(p => p.IsValid) ?? true)
        && !string.IsNullOrWhiteSpace(Title)
        && LifetimePerDays >= 0
        && Price >= 0;
}