using Oland.Odnoklassniki.Enums;

namespace Oland.Odnoklassniki.Rest;

/// <summary>
/// Представляет набор параметров для формирования запроса к REST API Одноклассников.
/// Позволяет гибко конфигурировать параметры запроса с помощью цепочки вызовов (fluent interface).
/// </summary>
/// <remarks>
/// Класс инкапсулирует логику построения параметров для методов API OK.ru, обеспечивая:
/// <list type="bullet">
/// <item>Типизированное добавление стандартных параметров (поля, пагинация, идентификаторы);</item>
/// <item>Валидацию значений на стороне клиента (диапазоны, обязательность);</item>
/// <item>Поддержку произвольных параметров через <see cref="InsertCustomParameter"/>;</item>
/// <item>Автоматическое преобразование значений в строковый формат для передачи в HTTP-запросе.</item>
/// </list>
/// 
/// <para><b>Пример использования:</b></para>
/// <code>
/// var parameters = new RestParameters()
///     .InsertFields("uid", "first_name", "last_name")
///     .InsertCount(20)
///     .InsertGroupId("123456")
///     .InsertAnchor("abc123");
/// </code>
/// 
/// <para><b>Важно:</b></para>
/// <list type="bullet">
/// <item>Класс не выполняет валидацию бизнес-логики (существование ID, права доступа) — это задача сервера;</item>
/// <item>Параметры пагинации (<c>anchor</c>, <c>pagingDirection</c>) зависят от конкретного метода API;</item>
/// <item>При слиянии параметров через <see cref="MergeParameters"/> существующие ключи перезаписываются.</item>
/// </list>
/// </remarks>
public class RestParameters
{
    private readonly Dictionary<string, object> _parameters = [];

    /// <summary>
    /// Добавляет параметр "fields" в запрос, указывая список запрашиваемых полей ответа.
    /// </summary>
    /// <remarks>
    /// Формирует параметр <c>fields</c> со списком имён полей, разделённых запятыми.
    /// Пример: <c>fields = ["uid", "first_name"]</c> → <c>fields=uid,first_name</c>.
    /// 
    /// <para><b>Примечания:</b></para>
    /// <list type="bullet">
    /// <item>Список доступных полей зависит от метода API и типа возвращаемого объекта;</item>
    /// <item>Запрос только необходимых полей снижает объём передаваемых данных и ускоряет ответ;</item>
    /// <item>Если параметр не указан, сервер возвращает набор полей по умолчанию (минимальный).</item>
    /// </list>
    /// </remarks>
    /// <param name="fields">
    /// Коллекция имён полей для включения в ответ API.
    /// Допустимые значения определяются документацией конкретного метода (например, <c>users.getInfo</c>).
    /// Может быть <see langword="null"/> или пустой — в этом случае метод не добавляет параметр.
    /// </param>
    /// <returns>Текущий экземпляр <see cref="RestParameters"/> для поддержки цепочки вызовов.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// Возникает, если любой элемент коллекции <paramref name="fields"/> равен <see langword="null"/>.
    /// </exception>
    public RestParameters InsertFields(params ICollection<string>? fields)
    {
        if (fields == null || fields.Count == 0)
        {
            return this;
        }
        
        _parameters["fields"] = string.Join(',', fields);

        return this;
    }

    /// <summary>
    /// Добавляет параметр "count", ограничивающий количество возвращаемых элементов в ответе.
    /// </summary>
    /// <remarks>
    /// Используется для пагинации результатов: сервер возвращает не более указанного числа записей.
    /// Пример: <c>count=50</c> → максимум 50 элементов в одной странице ответа.
    /// 
    /// <para><b>Ограничения:</b></para>
    /// <list type="bullet">
    /// <item>Допустимый диапазон: от 1 до 100 включительно;</item>
    /// <item>Значения вне диапазона вызывают исключение на стороне клиента;</item>
    /// <item>Сервер может вернуть меньше элементов, если доступно меньше запрошенного.</item>
    /// </list>
    /// </remarks>
    /// <param name="count">
    /// Количество элементов для возврата. Должно быть в диапазоне от 1 до 100.
    /// </param>
    /// <returns>Текущий экземпляр <see cref="RestParameters"/> для поддержки цепочки вызовов.</returns>
    /// <exception cref="System.ArgumentException">
    /// Возникает, если значение <paramref name="count"/> меньше 1 или больше 100.
    /// </exception>
    public RestParameters InsertCount(int count)
    {
        if (count <= 0 || count > 100)
        {
            throw new ArgumentException("The count must be in the range from 0 to 100");
        }

        _parameters["count"] = count;

        return this;
    }

    /// <summary>
    /// Добавляет произвольный параметр с указанным ключом и значением.
    /// </summary>
    /// <remarks>
    /// Метод позволяет расширять запрос параметрами, не покрытыми типизированными методами.
    /// Значение преобразуется в строку через <see cref="object.ToString"/> при слиянии параметров.
    /// 
    /// <para><b>Примеры использования:</b></para>
    /// <list type="bullet">
    /// <item><c>InsertCustomParameter("access_token", "abc123")</c>;</item>
    /// <item><c>InsertCustomParameter("photo_ids", "123,456,789")</c>;</item>
    /// <item><c>InsertCustomParameter("need_photo", true)</c> → <c>need_photo=True</c>.</item>
    /// </list>
    /// 
    /// <para><b>Важно:</b></para>
    /// <list type="bullet">
    /// <item>Ключи параметров чувствительны к регистру — должны соответствовать спецификации API;</item>
    /// <item>При добавлении дублирующегося ключа предыдущее значение перезаписывается;</item>
    /// <item>Не используйте метод для добавления параметров, имеющих типизированные обёртки.</item>
    /// </list>
    /// </remarks>
    /// <param name="key">Имя параметра (ключ) в соответствии со спецификацией API OK.ru.</param>
    /// <param name="value">
    /// Значение параметра любого типа. Будет преобразовано в строку при формировании запроса.
    /// Для коллекций рекомендуется предварительно форматировать значение (например, через <c>string.Join</c>).
    /// </param>
    /// <returns>Текущий экземпляр <see cref="RestParameters"/> для поддержки цепочки вызовов.</returns>
    /// <exception cref="System.ArgumentException">
    /// Возникает, если <paramref name="key"/> равен <see langword="null"/> или пустой строке.
    /// </exception>
    public RestParameters InsertCustomParameter(string key, object? value) 
    {
        if (value != null)
        {
            _parameters[key] = value;
        }

        return this;
    }

    /// <summary>
    /// Добавляет идентификатор группы в параметры запроса.
    /// </summary>
    /// <remarks>
    /// Формирует параметр <c>gid</c> с идентификатором целевой группы.
    /// Используется в методах, требующих указания группы для операции
    /// (например, <c>groups.getMembers</c>, <c>photos.getAlbums</c>).
    /// 
    /// <para><b>Пример:</b></para>
    /// <code>InsertGroupId("123456")</code> → <c>gid=123456</c>
    /// 
    /// <para><b>Важно:</b></para>
    /// <list type="bullet">
    /// <item>Если <paramref name="groupId"/> пуст или <see langword="null"/>, параметр не добавляется;</item>
    /// <item>Идентификатор должен соответствовать формату OK.ru (числовая строка);</item>
    /// <item>Невалидный ID приведёт к ошибке на стороне сервера, а не клиента.</item>
    /// </list>
    /// </remarks>
    /// <param name="groupId">
    /// Идентификатор группы в системе Одноклассников.
    /// Может быть пустым — в этом случае параметр не добавляется в запрос.
    /// </param>
    /// <returns>Текущий экземпляр <see cref="RestParameters"/> для поддержки цепочки вызовов.</returns>
    public RestParameters InsertGroupId(string groupId)
    {
        if (!string.IsNullOrEmpty(groupId))
        {
            _parameters["gid"] = groupId;
        }

        return this;
    }

    /// <summary>
    /// Добавляет текстовое описание в параметры запроса.
    /// </summary>
    /// <remarks>
    /// Формирует параметр <c>description</c> с произвольным текстом.
    /// Используется в методах создания или редактирования сущностей
    /// (например, <c>photos.createAlbum</c>, <c>posts.create</c>).
    /// 
    /// <para><b>Ограничения:</b></para>
    /// <list type="bullet">
    /// <item>Значение не может быть пустым или <see langword="null"/>;</item>
    /// <item>Максимальная длина определяется сервером (обычно до 1000 символов);</item>
    /// <item>Специальные символы экранируются автоматически при отправке запроса.</item>
    /// </list>
    /// </remarks>
    /// <param name="description">
    /// Текстовое описание для создаваемой или редактируемой сущности.
    /// Обязательное непустое значение.
    /// </param>
    /// <returns>Текущий экземпляр <see cref="RestParameters"/> для поддержки цепочки вызовов.</returns>
    /// <exception cref="System.ArgumentException">
    /// Возникает, если <paramref name="description"/> равен <see langword="null"/> или пустой строке.
    /// </exception>
    public RestParameters InsertDescription(string description)
    {
        if (string.IsNullOrEmpty(description))
        {
            throw new ArgumentException("Description can not be null or empty", nameof(description));
        }

        _parameters["description"] = description;

        return this;
    }

    /// <summary>
    /// Добавляет идентификатор фотографии в параметры запроса.
    /// </summary>
    /// <remarks>
    /// Формирует параметр <c>photo_id</c> с идентификатором целевой фотографии.
    /// Используется в методах работы с изображениями
    /// (например, <c>photos.getInfo</c>, <c>photos.remove</c>, <c>photos.edit</c>).
    /// 
    /// <para><b>Важно:</b></para>
    /// <list type="bullet">
    /// <item>Значение не может быть пустым или <see langword="null"/>;</item>
    /// <item>Идентификатор должен соответствовать формату OK.ru (числовая строка);</item>
    /// <item>Попытка использовать несуществующий ID приведёт к ошибке сервера.</item>
    /// </list>
    /// </remarks>
    /// <param name="photoId">
    /// Уникальный идентификатор фотографии в системе Одноклассников.
    /// Обязательное непустое значение.
    /// </param>
    /// <returns>Текущий экземпляр <see cref="RestParameters"/> для поддержки цепочки вызовов.</returns>
    /// <exception cref="System.ArgumentException">
    /// Возникает, если <paramref name="photoId"/> равен <see langword="null"/> или пустой строке.
    /// </exception>
    public RestParameters InsertPhotoId(string? photoId)
    {
        if (!string.IsNullOrEmpty(photoId))
        {
            _parameters["photo_id"] = photoId;
        }

        return this;
    }

    public RestParameters InsertCatalogId(string catalogId)
    {
        if (string.IsNullOrEmpty(catalogId))
        {
            throw new ArgumentException("Catalog Id can not be null or empty", nameof(catalogId));
        }
        
        _parameters["catalog_id"] = catalogId;
        
        return this;
    }
    
    public RestParameters InsertTab(string tab)
    {
        if (string.IsNullOrEmpty(tab))
        {
            throw new ArgumentException("Tab can not be null or empty", nameof(tab));
        }
        
        _parameters["tab"] = tab;
        
        return this;
    }
    
    public RestParameters InsertProductId(string productId)
    {
        if (string.IsNullOrEmpty(productId))
        {
            throw new ArgumentException("Product Id can not be null or empty", nameof(productId));
        }
        
        _parameters["product_id"] = productId;
        
        return this;
    }
    
    public RestParameters InsertAttachment(string attachment)
    {
        if (string.IsNullOrEmpty(attachment))
        {
            throw new ArgumentException("attachment can not be null or empty", nameof(attachment));
        }
        
        _parameters["attachment"] = attachment;
        
        return this;
    }
    
    public RestParameters InsertProductIds(IEnumerable<string> productIds)
    {
        if (productIds == null || !productIds.Any())
        {
            throw new ArgumentException("Product Ids can not be null or empty", nameof(productIds));
        }
        
        _parameters["product_ids"] = string.Join(',', productIds);
        
        return this;
    }
    
    public RestParameters InsertCatalogIds(IEnumerable<string> catalogIds)
    {
        _parameters["catalog_ids"] = string.Join(',', catalogIds.ToArray());
        
        return this;
    }

    public RestParameters InsertAdminRestricted(bool adminRestricted)
    {
        _parameters["admin_restricted"] = adminRestricted.ToString();

        return this;
    }
    
    /// <summary>
    /// Добавляет заголовок (название) в параметры запроса.
    /// </summary>
    /// <remarks>
    /// Формирует параметр <c>title</c> с текстовым названием сущности.
    /// Используется в методах создания или редактирования
    /// (например, <c>photos.createAlbum</c>, <c>groups.edit</c>).
    /// 
    /// <para><b>Ограничения:</b></para>
    /// <list type="bullet">
    /// <item>Значение не может быть пустым или <see langword="null"/>;</item>
    /// <item>Максимальная длина определяется сервером (обычно до 255 символов);</item>
    /// <item>Поддерживаются кириллические и латинские символы, пробелы, спецсимволы.</item>
    /// </list>
    /// </remarks>
    /// <param name="title">
    /// Заголовок создаваемой или редактируемой сущности.
    /// Обязательное непустое значение.
    /// </param>
    /// <returns>Текущий экземпляр <see cref="RestParameters"/> для поддержки цепочки вызовов.</returns>
    /// <exception cref="System.ArgumentException">
    /// Возникает, если <paramref name="title"/> равен <see langword="null"/> или пустой строке.
    /// </exception>
    public RestParameters InsertTitle(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentException("Title can not be null or empty", nameof(title));
        }

        _parameters["title"] = title;

        return this;
    }

    public RestParameters InsertName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Title can not be null or empty", nameof(name));
        }
        
        _parameters["name"] = name;

        return this;
    }

    /// <summary>
    /// Добавляет идентификатор друга в параметры запроса.
    /// </summary>
    /// <remarks>
    /// Формирует параметр <c>fid</c> с идентификатором пользователя в контексте операций с друзьями.
    /// Используется в методах управления списком друзей
    /// (например, <c>friends.add</c>, <c>friends.remove</c>, <c>friends.check</c>).
    /// 
    /// <para><b>Примечания:</b></para>
    /// <list type="bullet">
    /// <item>Если <paramref name="friendId"/> пуст или <see langword="null"/>, параметр не добавляется;</item>
    /// <item>Идентификатор должен соответствовать формату пользователя OK.ru;</item>
    /// <item>Операции с друзьями требуют соответствующих прав доступа токена.</item>
    /// </list>
    /// </remarks>
    /// <param name="friendId">
    /// Идентификатор пользователя в системе Одноклассников.
    /// Может быть пустым — в этом случае параметр не добавляется в запрос.
    /// </param>
    /// <returns>Текущий экземпляр <see cref="RestParameters"/> для поддержки цепочки вызовов.</returns>
    public RestParameters InsertFriendId(string friendId)
    {
        if (!string.IsNullOrEmpty(friendId))
        {
            _parameters["fid"] = friendId;
        }

        return this;
    }

    /// <summary>
    /// Добавляет анкор пагинации в параметры запроса.
    /// </summary>
    /// <remarks>
    /// Формирует параметр <c>pagingAnchor</c> для cursor-based пагинации.
    /// Используется для загрузки следующей или предыдущей страницы результатов.
    /// 
    /// <para><b>Сценарий использования:</b></para>
    /// <list type="number">
    /// <item>Получить ответ с пагинацией (содержит поле <c>anchor</c>);</item>
    /// <item>Передать значение анкора в <see cref="InsertPagingAnchor"/> для следующего запроса;</item>
    /// <item>Повторять до достижения конца выборки (<c>hasMore = false</c>).</item>
    /// </list>
    /// 
    /// <para><b>Важно:</b></para>
    /// <list type="bullet">
    /// <item>Анкор действителен только для текущей сессии и конкретного метода;</item>
    /// <item>Пустой анкор (<c>""</c>) означает начало выборки (первая страница);</item>
    /// <item>Не рекомендуется кэшировать анкору между разными вызовами API.</item>
    /// </list>
    /// </remarks>
    /// <param name="anchor">
    /// Строковое значение курсора пагинации, полученное из предыдущего ответа.
    /// Может быть пустым — в этом случае параметр не добавляется в запрос.
    /// </param>
    /// <returns>Текущий экземпляр <see cref="RestParameters"/> для поддержки цепочки вызовов.</returns>
    public RestParameters InsertPagingAnchor(string anchor)
    {
        if (!string.IsNullOrEmpty(anchor))
        {
            _parameters["pagingAnchor"] = anchor;
        }

        return this;
    }

    /// <summary>
    /// Добавляет направление пагинации в параметры запроса.
    /// </summary>
    /// <remarks>
    /// Формирует параметр <c>pagingDirection</c> со строковым представлением перечисления <see cref="PagingDirection"/>.
    /// Используется для управления порядком загрузки данных относительно текущей позиции.
    /// 
    /// <para><b>Возможные значения:</b></para>
    /// <list type="bullet">
    /// <item><c>FORWARD</c> — загрузка следующих записей (вперёд по хронологии);</item>
    /// <item><c>BACKWARD</c> — загрузка предыдущих записей (назад по хронологии);</item>
    /// <item><c>AROUND</c> — загрузка записей вокруг текущей позиции.</item>
    /// </list>
    /// 
    /// <para><b>Важно:</b></para>
    /// <list type="bullet">
    /// <item>Поддержка направления зависит от конкретного метода API;</item>
    /// <item>Значение по умолчанию определяется сервером, если параметр не указан;</item>
    /// <item>Некорректное направление может привести к пустому результату.</item>
    /// </list>
    /// </remarks>
    /// <param name="direction">
    /// Направление навигации в перечислении <see cref="PagingDirection"/>.
    /// </param>
    /// <returns>Текущий экземпляр <see cref="RestParameters"/> для поддержки цепочки вызовов.</returns>
    public RestParameters InsertPagingDirection(PagingDirection direction)
    {
        _parameters["pagingDirection"] = direction.ToString();

        return this;
    }

    /// <summary>
    /// Добавляет общее направление сортировки/навигации в параметры запроса.
    /// </summary>
    /// <remarks>
    /// Формирует параметр <c>direction</c> со строковым представлением перечисления <see cref="PagingDirection"/>.
    /// Используется в методах, где направление влияет на порядок результатов
    /// (например, сортировка по дате, рейтингу, алфавиту).
    /// 
    /// <para><b>Отличие от <see cref="InsertPagingDirection"/>:</b></para>
    /// <list type="bullet">
    /// <item><c>direction</c> — влияет на сортировку результатов;</item>
    /// <item><c>pagingDirection</c> — влияет на навигацию по страницам пагинации;</item>
    /// <item>Некоторые методы API поддерживают только один из параметров.</item>
    /// </list>
    /// </remarks>
    /// <param name="direction">
    /// Направление в перечислении <see cref="PagingDirection"/>.
    /// </param>
    /// <returns>Текущий экземпляр <see cref="RestParameters"/> для поддержки цепочки вызовов.</returns>
    public RestParameters InsertDirection(PagingDirection direction)
    {
        _parameters["direction"] = direction.ToString();

        return this;
    }

    /// <summary>
    /// Добавляет анкор навигации в параметры запроса.
    /// </summary>
    /// <remarks>
    /// Формирует параметр <c>anchor</c> для cursor-based навигации.
    /// Является альтернативой <see cref="InsertPagingAnchor"/> для методов,
    /// использующих упрощённое имя параметра.
    /// 
    /// <para><b>Важно:</b></para>
    /// <list type="bullet">
    /// <item>Пустой анкор (<c>""</c>) означает начало выборки;</item>
    /// <item>Анкор должен быть получен из ответа предыдущего запроса;</item>
    /// <item>Не смешивайте <c>anchor</c> и <c>pagingAnchor</c> в одном запросе — используйте тот, что указан в документации метода.</item>
    /// </list>
    /// </remarks>
    /// <param name="anchor">
    /// Строковое значение курсора навигации.
    /// Может быть пустым — в этом случае параметр не добавляется в запрос.
    /// </param>
    /// <returns>Текущий экземпляр <see cref="RestParameters"/> для поддержки цепочки вызовов.</returns>
    public RestParameters InsertAnchor(string anchor)
    {
        if (!string.IsNullOrEmpty(anchor))
        {
            _parameters["anchor"] = anchor;
        }

        return this;
    }

    /// <summary>
    /// Добавляет идентификатор альбома в параметры запроса.
    /// </summary>
    /// <remarks>
    /// Формирует параметр <c>aid</c> (album ID) с идентификатором целевого альбома.
    /// Используется в методах работы с фотоальбомами
    /// (например, <c>photos.get</c>, <c>photos.editAlbum</c>, <c>photos.removeAlbum</c>).
    /// 
    /// <para><b>Важно:</b></para>
    /// <list type="bullet">
    /// <item>Значение не может быть пустым или <see langword="null"/>;</item>
    /// <item>Идентификатор должен соответствовать формату альбома OK.ru;</item>
    /// <item>Попытка использовать несуществующий или недоступный ID приведёт к ошибке сервера.</item>
    /// </list>
    /// </remarks>
    /// <param name="albumId">
    /// Уникальный идентификатор альбома в системе Одноклассников.
    /// Обязательное непустое значение.
    /// </param>
    /// <returns>Текущий экземпляр <see cref="RestParameters"/> для поддержки цепочки вызовов.</returns>
    /// <exception cref="System.ArgumentException">
    /// Возникает, если <paramref name="albumId"/> равен <see langword="null"/> или пустой строке.
    /// </exception>
    public RestParameters InsertAlbumId(string albumId)
    {
        if (string.IsNullOrEmpty(albumId))
        {
            throw new ArgumentException("Album Id can not be null or empty", nameof(albumId));
        }

        _parameters["aid"] = albumId;

        return this;
    }

    /// <summary>
    /// Сливает внутренние параметры с переданным словарём, преобразуя значения в строки, и возвращает результирующий словарь.
    /// </summary>
    /// <remarks>
    /// Метод выполняет финальную подготовку параметров для отправки в HTTP-запросе:
    /// <list type="bullet">
    /// <item>Все значения из внутреннего хранилища преобразуются в строки через <see cref="object.ToString"/>;</item>
    /// <item>Значения <see langword="null"/> заменяются на пустую строку;</item>
    /// <item>Существующие ключи в <paramref name="externalParameters"/> перезаписываются значениями из внутренних параметров;</item>
    /// <item>Новые ключи добавляются в словарь.</item>
    /// </list>
    /// 
    /// <para><b>Особенности реализации:</b></para>
    /// <list type="bullet">
    /// <item>Параметр <paramref name="externalParameters"/> передаётся с модификатором <c>in</c>, но так как <see cref="Dictionary{TKey, TValue}"/> — ссылочный тип, словарь изменяется напрямую;</item>
    /// <item>Метод возвращает ссылку на тот же словарь для поддержки цепочки вызовов;</item>
    /// <item>Порядок ключей в результирующем словаре не гарантируется.</item>
    /// </list>
    /// 
    /// <para><b>Пример использования:</b></para>
    /// <code>
    /// var external = new Dictionary&lt;string, string&gt; { ["method"] = "users.getInfo" };
    /// var parameters = new RestParameters()
    ///     .InsertFields("uid", "first_name")
    ///     .InsertCount(10);
    /// var result = parameters.MergeParameters(external);
    /// // result: { ["method"]="users.getInfo", ["fields"]="uid,first_name", ["count"]="10" }
    /// </code>
    /// </remarks>
    /// <param name="externalParameters">
    /// Исходный словарь параметров, который будет модифицирован:
    /// существующие ключи могут быть перезаписаны, новые — добавлены на основе внутренних параметров.
    /// </param>
    /// <returns>
    /// Ссылка на модифицированный словарь <paramref name="externalParameters"/> с добавленными параметрами.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// Возникает, если <paramref name="externalParameters"/> равен <see langword="null"/>.
    /// </exception>
    public IDictionary<string, string> MergeParameters(in Dictionary<string, string> externalParameters)
    {
        foreach (var (key, value) in _parameters)
        {
            externalParameters[key] = value?.ToString() ?? string.Empty;
        }

        return externalParameters;
    }
}