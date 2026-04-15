using Oland.MediaManager.Domain.MediaItems;
using Oland.MediaManager.Domain.MediaItems.App;
using Oland.MediaManager.Domain.MediaItems.Link;
using Oland.MediaManager.Domain.MediaItems.Movie;
using Oland.MediaManager.Domain.MediaItems.Music;
using Oland.MediaManager.Domain.MediaItems.Photo;
using Oland.MediaManager.Domain.MediaItems.Poll;
using Oland.MediaManager.Domain.MediaItems.Text;

namespace Oland.MediaManager.Application.Builders;

/// <summary>
/// Конструктор для сборки коллекции разнотипных медиа-объектов по паттерну Builder.
/// Поддерживает добавление фото, видео, музыки, опросов, ссылок, текста, приложений и товаров.
/// </summary>
public class MediaCollectionBuilder
{
    private readonly List<MediaItem> _media = [];

    /// <summary>
    /// Добавляет медиа-элемент типа «Фото» в коллекцию.
    /// </summary>
    /// <param name="configure">
    /// Опциональный делегат для настройки вложенного <see cref="PhotoBuilder"/>.
    /// </param>
    /// <returns>Текущий экземпляр билдера для цепочки вызовов.</returns>
    public MediaCollectionBuilder AddPhoto(Action<PhotoBuilder>? configure = null)
    {
        var builder = new PhotoBuilder();
        configure?.Invoke(builder);
        _media.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Добавляет медиа-элемент типа «Фильм» по списку идентификаторов.
    /// </summary>
    /// <param name="movieIds">
    /// Массив внешних идентификаторов фильмов (например, из каталога контента).
    /// </param>
    /// <returns>Текущий экземпляр билдера для цепочки вызовов.</returns>
    public MediaCollectionBuilder AddMovie(params string[] movieIds)
    {
        var media = new MovieMedia();
        media.List.AddRange(movieIds.Select(id => new MovieItem { Id = id }));
        _media.Add(media);
        return this;
    }

    /// <summary>
    /// Добавляет медиа-элемент типа «Музыка» с настройкой через делегат.
    /// </summary>
    /// <param name="configure">
    /// Обязательный делегат для настройки вложенного <see cref="MusicBuilder"/>.
    /// </param>
    /// <returns>Текущий экземпляр билдера для цепочки вызовов.</returns>
    public MediaCollectionBuilder AddMusic(Action<MusicBuilder> configure)
    {
        var builder = new MusicBuilder();
        configure(builder);
        _media.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Добавляет медиа-элемент типа «Опрос» с вопросом и настройкой ответов.
    /// </summary>
    /// <param name="question">Текст вопроса опроса. Обязательное поле.</param>
    /// <param name="configure">
    /// Делегат для настройки вложенного <see cref="PollBuilder"/> (ответы, опции).
    /// </param>
    /// <returns>Текущий экземпляр билдера для цепочки вызовов.</returns>
    public MediaCollectionBuilder AddPoll(string question, Action<PollBuilder> configure)
    {
        var builder = new PollBuilder { Question = question };
        configure(builder);
        _media.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Добавляет медиа-элемент типа «Ссылка».
    /// </summary>
    /// <param name="url">
    /// Целевой URL (HTTP/HTTPS). Рекомендуется валидировать формат перед вызовом.
    /// </param>
    /// <returns>Текущий экземпляр билдера для цепочки вызовов.</returns>
    public MediaCollectionBuilder AddLink(string url)
    {
        _media.Add(new LinkMedia { Url = url });
        return this;
    }

    /// <summary>
    /// Добавляет медиа-элемент типа «Текст», если значение не пустое.
    /// </summary>
    /// <param name="text">Текстовое содержимое. Пустые и null-значения игнорируются.</param>
    /// <returns>Текущий экземпляр билдера для цепочки вызовов.</returns>
    public MediaCollectionBuilder AddText(string? text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            _media.Add(new TextMedia { Text = text });
        }
        
        return this;
    }

    /// <summary>
    /// Добавляет медиа-элемент типа «Приложение» с настройкой действий и изображений.
    /// </summary>
    /// <param name="configure">
    /// Опциональный делегат для настройки вложенного <see cref="AppBuilder"/>.
    /// </param>
    /// <returns>Текущий экземпляр билдера для цепочки вызовов.</returns>
    public MediaCollectionBuilder AddApp(Action<AppBuilder>? configure = null)
    {
        var builder = new AppBuilder();
        configure?.Invoke(builder);
        _media.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Добавляет медиа-элемент типа «Товар» с параметрами монетизации.
    /// </summary>
    /// <param name="price">Цена товара. Обязательное поле.</param>
    /// <param name="partnerLink">
    /// Опциональная партнёрская ссылка для перехода после покупки.
    /// </param>
    /// <param name="lifetime">
    /// Срок действия предложения в днях. По умолчанию — 90.
    /// </param>
    /// <param name="currency">
    /// Код валюты (ISO 4217, например «RUB», «USD»). Если не указан, используется валюта по умолчанию системы.
    /// </param>
    /// <returns>Текущий экземпляр билдера для цепочки вызовов.</returns>
    public MediaCollectionBuilder AddProduct(decimal price, string? partnerLink = null, int lifetime = 90,
        string? currency = null)
    {
        var product = new ProductMedia()
        {
            Currency = currency,
            Lifetime = lifetime,
            PartnerLink = partnerLink ,
            Price = price
        };
        
        _media.Add(product);

        return this;
    }

    /// <summary>
    /// Завершает сборку и возвращает неизменяемую коллекцию медиа-элементов.
    /// </summary>
    /// <returns>Экземпляр <see cref="MediaCollection"/> с накопленными элементами.</returns>
    public MediaCollection Build()
    {
        return new MediaCollection(_media.ToList());
    }

    /// <summary>
    /// Встроенный билдер для создания медиа-элемента «Фото».
    /// </summary>
    public class PhotoBuilder
    {
        private readonly List<PhotoItem> _items = new();

        /// <summary>
        /// Добавляет фото по внешнему идентификатору (например, из медиатеки).
        /// </summary>
        /// <param name="id">Уникальный идентификатор фото. Обязательное поле.</param>
        /// <returns>Текущий экземпляр билдера для цепочки вызовов.</returns>
        public PhotoBuilder AddById(string id)
        {
            _items.Add(new PhotoItem { Id = id });
            return this;
        }

        /// <summary>
        /// Собирает и возвращает готовый <see cref="PhotoMedia"/>.
        /// </summary>
        /// <returns>Экземпляр <see cref="PhotoMedia"/> со списком фото.</returns>
        internal PhotoMedia Build()
        {
            return new PhotoMedia { List = _items };
        }
    }

    /// <summary>
    /// Встроенный билдер для создания медиа-элемента «Музыка».
    /// </summary>
    public class MusicBuilder
    {
        private readonly List<MusicItem> _items = new();

        /// <summary>
        /// Добавляет музыкальный трек с метаданными.
        /// </summary>
        /// <param name="id">Внешний идентификатор трека. Обязательное поле.</param>
        /// <param name="title">Опциональное название трека.</param>
        /// <param name="artist">Опциональное имя исполнителя.</param>
        /// <param name="album">Опциональное название альбома.</param>
        /// <returns>Текущий экземпляр билдера для цепочки вызовов.</returns>
        public MusicBuilder AddTrack(string id, string? title = null,
            string? artist = null, string? album = null)
        {
            _items.Add(new MusicItem
            {
                Id = id,
                Title = title,
                ArtistName = artist,
                AlbumName = album
            });
            return this;
        }

        /// <summary>
        /// Собирает и возвращает готовый <see cref="MusicMedia"/>.
        /// </summary>
        /// <returns>Экземпляр <see cref="MusicMedia"/> со списком треков.</returns>
        internal MusicMedia Build()
        {
            return new MusicMedia { List = _items };
        }
    }

    /// <summary>
    /// Встроенный билдер для создания медиа-элемента «Опрос».
    /// </summary>
    public class PollBuilder
    {
        private readonly List<PollAnswer> _answers = new();
        
        /// <summary>
        /// Текст вопроса опроса. Задаётся через метод <see cref="MediaCollectionBuilder.AddPoll"/>.
        /// </summary>
        public string Question { get; internal set; } = string.Empty;
        
        /// <summary>
        /// Опциональные предустановленные варианты ответов в виде CSV-строки.
        /// Задаётся через метод <see cref="WithOptions"/>.
        /// </summary>
        public string? Options { get; set; }

        /// <summary>
        /// Добавляет вариант ответа в опрос.
        /// </summary>
        /// <param name="text">Текст варианта ответа. Обязательное поле.</param>
        /// <returns>Текущий экземпляр билдера для цепочки вызовов.</returns>
        public PollBuilder AddAnswer(string text)
        {
            _answers.Add(new PollAnswer { Text = text });
            return this;
        }

        /// <summary>
        /// Устанавливает предустановленные варианты ответов как строку, разделённую запятыми.
        /// </summary>
        /// <param name="options">Массив текстов вариантов.</param>
        /// <returns>Текущий экземпляр билдера для цепочки вызовов.</returns>
        public PollBuilder WithOptions(params string[] options)
        {
            Options = string.Join(",", options);
            return this;
        }

        /// <summary>
        /// Собирает и возвращает готовый <see cref="PollMedia"/>.
        /// </summary>
        /// <returns>Экземпляр <see cref="PollMedia"/> с вопросом и ответами.</returns>
        internal PollMedia Build()
        {
            return new PollMedia
            {
                Question = Question,
                Answers = _answers,
                Options = Options
            };
        }
    }

    /// <summary>
    /// Встроенный билдер для создания медиа-элемента «Приложение».
    /// </summary>
    public class AppBuilder
    {
        private readonly List<AppAction> _actions = new();
        private readonly List<AppImage> _images = new();
        
        /// <summary>
        /// Опциональный текстовый блок приложения.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Устанавливает текстовое описание приложения.
        /// </summary>
        /// <param name="text">Текст для отображения в интерфейсе.</param>
        /// <returns>Текущий экземпляр билдера для цепочки вызовов.</returns>
        public AppBuilder WithText(string text)
        {
            Text = text;
            return this;
        }

        /// <summary>
        /// Добавляет изображение в интерфейс приложения.
        /// </summary>
        /// <param name="url">URL изображения (обязательное поле).</param>
        /// <param name="mark">Опциональная текстовая метка на изображении.</param>
        /// <param name="title">Опциональная всплывающая подсказка.</param>
        /// <returns>Текущий экземпляр билдера для цепочки вызовов.</returns>
        public AppBuilder AddImage(string url, string? mark = null, string? title = null)
        {
            _images.Add(new AppImage { Url = url, Mark = mark, Title = title });
            return this;
        }

        /// <summary>
        /// Добавляет интерактивное действие (кнопку) в интерфейс приложения.
        /// </summary>
        /// <param name="text">Текст на кнопке. Обязательное поле.</param>
        /// <param name="mark">Опциональный системный маркер для обработки действия.</param>
        /// <returns>Текущий экземпляр билдера для цепочки вызовов.</returns>
        public AppBuilder AddAction(string text, string? mark = null)
        {
            _actions.Add(new AppAction { Text = text, Mark = mark });
            return this;
        }

        /// <summary>
        /// Собирает и возвращает готовый <see cref="AppMedia"/>.
        /// </summary>
        /// <returns>Экземпляр <see cref="AppMedia"/> с текстом, изображениями и действиями.</returns>
        internal AppMedia Build()
        {
            return new AppMedia
            {
                Text = Text,
                Images = _images,
                Actions = _actions
            };
        }
    }
}