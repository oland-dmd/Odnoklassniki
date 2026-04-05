namespace Oland.Odnoklassniki.Rest.AnchorNavigators;

/// <summary>
/// Модель ответа для постраничной навигации на основе анкоров (cursor-based pagination).
/// Используется для передачи данных страниц и метаданных навигации от внешнего API (Odnoklassniki).
/// Совместима с навигатором <see cref="AnchorNavigator{TResponse}"/>.
/// </summary>
/// <typeparam name="TResponse">Тип данных элементов в коллекции результатов.</typeparam>
public class AnchorResponse<TResponse>
{
    /// <summary>
    /// Уникальный идентификатор (курсор) для загрузки следующей страницы данных.
    /// Используется для формирования запроса к следующей порции данных.
    /// Обязательное поле.
    /// </summary>
    public required string Anchor { get; init; }
    
    /// <summary>
    /// Общее количество доступных записей во всей выборке.
    /// Может отсутствовать (null), если подсчёт полного количества не поддерживается источником данных.
    /// </summary>
    public int? TotalCount { get; init; }
    
    /// <summary>
    /// Флаг наличия дополнительных страниц данных.
    /// <c>true</c> — существуют следующие записи, <c>false</c> — достигнут конец выборки.
    /// Обязательное поле для контроля цикла итерации.
    /// </summary>
    public bool HasMore { get; init; }
    
    /// <summary>
    /// Коллекция данных текущей страницы.
    /// Может быть пустой или отсутствовать (null), если на текущей странице нет результатов.
    /// </summary>
    public ICollection<TResponse>? Results { get; init; }
}