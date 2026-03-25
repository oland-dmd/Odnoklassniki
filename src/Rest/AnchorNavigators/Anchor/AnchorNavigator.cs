namespace Odnoklassniki.Rest.AnchorNavigators.Anchor;

/// <summary>
/// Реализует паттерн асинхронного перечисления для навигации по страницам данных через анкоры (anchor-based pagination).
/// Предназначен для последовательной загрузки результатов из внешнего API (Odnoklassniki) без полной предзагрузки данных.
/// </summary>
/// <typeparam name="TResponse">Тип данных, содержащихся в поле результатов ответа.</typeparam>
public class AnchorNavigator<TResponse>(Func<string, Task<AnchorResponse<TResponse>>> getResponse, string anchor = "")
    : IAsyncEnumerable<AnchorResponse<TResponse>>, IAsyncEnumerator<AnchorResponse<TResponse>>
{
    /// <summary>
    /// Сбрасывает состояние перечислителя к начальной позиции.
    /// </summary>
    /// <remarks>
    /// Инициализирует свойство <c>Current</c> новым объектом с пустым анкором и флагом <c>HasMore = true</c>.
    /// Используется для повторной итерации по тому же набору данных.
    /// </remarks>
    public void Reset()
    {
        Current = new AnchorResponse<TResponse>
        {
            Anchor = ""
        };
    }

    /// <summary>
    /// Выполняет переход к следующей странице данных.
    /// </summary>
    /// <remarks>
    /// Загружает данные через делегат <c>getResponse</c>, передавая анкор из текущего ответа.
    /// Итерация прекращается, если флаг <c>HasMore</c> установлен в false или список результатов пуст.
    /// </remarks>
    /// <returns>True, если следующая страница успешно загружена и содержит данные; иначе false.</returns>
    public async ValueTask<bool> MoveNextAsync()
    {
        if (!Current.HasMore) return false;
        
        Current = await getResponse(Current?.Anchor ?? string.Empty);
        
        return Current.Results?.Count > 0;

    }

    /// <summary>
    /// Текущий ответ от сервиса, содержащий данные страницы и метаданные навигации.
    /// </summary>
    /// <value>
    /// Экземпляр <see cref="AnchorResponse{TResponse}"/> с данными текущей итерации.
    /// Инициализируется начальным анкором при создании навигатора.
    /// </value>
    public AnchorResponse<TResponse> Current { get; private set; } = new()
    {
        Anchor = anchor,
        HasMore = true
    };

    /// <summary>
    /// Освобождает ресурсы, используемые навигатором.
    /// </summary>
    /// <remarks>
    /// В текущей реализации метод пуст, так как навигатор не удерживает неуправляемые ресурсы.
    /// </remarks>
    public async ValueTask DisposeAsync()
    {
    }

    /// <summary>
    /// Возвращает асинхронный перечислитель для использования в конструкции await foreach.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Экземпляр текущего навигатора.</returns>
    public IAsyncEnumerator<AnchorResponse<TResponse>> GetAsyncEnumerator(CancellationToken cancellationToken = new())
    {
        return this;
    }
}